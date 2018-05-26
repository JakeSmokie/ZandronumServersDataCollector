using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using EncodeLibrary.Huffman;

namespace ZandronumServersDataCollector {
    public class ServerListFetcher {
        private enum ResponseReadStates {
            WaitForNextServerListPart,

            End,
            ReadServerBlock
        }

        private readonly HuffmanCodec _huffmanCodec;

        public ServerListFetcher() {
            _huffmanCodec = new HuffmanCodec(HuffmanCodec.SkulltagCompatibleHuffmanTree);
        }

        public IEnumerable<IPEndPoint> FetchServerList(IEnumerable<(string host, int port)> masterServers) {
            var serverList = new List<IPEndPoint>();

            foreach (var masterServer in masterServers) {
                serverList.AddRange(FetchServerList(masterServer));
            }

            return serverList;
        }

        public IEnumerable<IPEndPoint> FetchServerList((string host, int port) masterServer) {
            var query = ConstructLauncherQuery();

            using (var udpClient = new UdpClient()) {
                udpClient.Connect(masterServer.host, masterServer.port);

                var sendTask = udpClient.SendAsync(query, query.Length);
                sendTask.Wait();

                IPEndPoint ip = null;

                var recievedData = new List<byte>();

                do {
                    var buffer = udpClient.Receive(ref ip);
                    recievedData.AddRange(buffer);
                } while (udpClient.Available != 0);

                // check for unencoded data
                var serverResponse =
                    recievedData[0] == 0xFF
                        ? recievedData.Skip(1).ToArray()
                        : _huffmanCodec.Decode(recievedData.ToArray());

                return ReadResponse(new BinaryReader(new MemoryStream(serverResponse)));
            }
        }

        private static IEnumerable<IPEndPoint> ReadResponse(BinaryReader serverResponse) {
            var state = ResponseReadStates.WaitForNextServerListPart;
            var packetNum = 0;

            while (state != ResponseReadStates.End) {
                switch (state) {
                    case ResponseReadStates.WaitForNextServerListPart:
                        // check for unencoded data
                        if (serverResponse.ReadByte() != 0xFF) {
                            serverResponse.BaseStream.Seek(-1, SeekOrigin.Current);
                        }

                        if (serverResponse.ReadInt32() == (int) MasterResponseCommands.BeginServerListPart) {
                            packetNum = serverResponse.ReadByte();
                            state = ResponseReadStates.ReadServerBlock;

                            if (serverResponse.ReadByte() != (int) MasterResponseCommands.ServerBlock) {
                                throw new ServerListFetcherException("");
                            }
                        }

                        break;
                    case ResponseReadStates.ReadServerBlock:
                        // is server block terminated
                        if (serverResponse.ReadByte() == 0) {
                            var responseCommand = serverResponse.ReadByte();

                            if (responseCommand == (int) MasterResponseCommands.EndServerList) {
                                state = ResponseReadStates.End;
                            }

                            if (responseCommand == (int) MasterResponseCommands.EndServerListPart) {
                                state = ResponseReadStates.WaitForNextServerListPart;
                            }

                            break;
                        }

                        // we read 1 byte so we need to seek 1 byte backward
                        serverResponse.BaseStream.Seek(-1, SeekOrigin.Current);

                        var portsAmount = serverResponse.ReadByte();
                        var ip = serverResponse.ReadUInt32();

                        for (var i = 0; i < portsAmount; i++) {
                            yield return new IPEndPoint(ip, serverResponse.ReadUInt16());
                        }

                        break;
                    case ResponseReadStates.End:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private byte[] ConstructLauncherQuery() {
            var message = new List<byte>();

            message.AddRange(BitConverter.GetBytes((int) MasterServerCommands.LauncherMasterChallenge));
            message.AddRange(BitConverter.GetBytes((short) MasterServerVersion.UsualVersion));

            return _huffmanCodec.Encode(message.ToArray());
        }
    }
}