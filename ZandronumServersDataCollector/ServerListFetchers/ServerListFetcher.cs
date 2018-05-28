using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using EncodeLibrary.Huffman;
using ZandronumServersDataCollector.Extensions;

namespace ZandronumServersDataCollector.ServerListFetchers {
    public partial class ServerListFetcher {
        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        private const int Timeout = 2000;

        private const int AttemptsAmount = 5;

        private static readonly HuffmanCodec
            HuffmanCodec = new HuffmanCodec(HuffmanCodec.SkulltagCompatibleHuffmanTree);

        public IEnumerable<IPEndPoint> FetchServerList(IEnumerable<(string host, int port)> masterServers) {
            var serverList = new List<IPEndPoint>();

            foreach (var masterServer in masterServers) {
                serverList.AddRange(FetchServerList(masterServer));
            }

            return serverList;
        }

        public IEnumerable<IPEndPoint> FetchServerList((string host, int port) masterServer) {
            using (var socket = new Socket(SocketType.Dgram, ProtocolType.Udp) {
                ReceiveTimeout = 1000
            }) {
                ConnectAndSendQuery(masterServer, socket);
                return ReadResponse(socket).ToArray();
            }
        }

        private static void ConnectAndSendQuery((string host, int port) masterServer, Socket socket) {
            socket.Connect(masterServer.host, masterServer.port);
            var query = ConstructLauncherQuery();

            socket.Send(query, SocketFlags.None);
        }

        private static byte[] ConstructLauncherQuery() {
            var message = new List<byte>();

            message.AddRange(BitConverter.GetBytes((int) MasterServerCommands.LauncherMasterChallenge));
            message.AddRange(BitConverter.GetBytes((short) MasterServerVersion.UsualVersion));

            return HuffmanCodec.Encode(message.ToArray());
        }

        private static IEnumerable<IPEndPoint> ReadResponse(Socket socket) {
            BinaryReader serverResponse = null;
            var state = ResponseReadStates.WaitForNextServerListPart;

            while (state != ResponseReadStates.End) {
                switch (state) {
                    case ResponseReadStates.WaitForNextServerListPart:
                        if (!socket.Connected)
                            yield break;

                        serverResponse = WaitForNextServerListPart(socket);
                        state = ResponseReadStates.ReadServerBlock;

                        break;
                    case ResponseReadStates.ReadServerBlock:
                        // is server block terminated
                        if (serverResponse.ReadByte() == 0) {
                            state = FindOutIsItEndOfServerList(serverResponse);
                            break;
                        }

                        foreach (var ipEndPoint in ReadServers(serverResponse))
                            yield return ipEndPoint;

                        break;
                    case ResponseReadStates.End:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static BinaryReader WaitForNextServerListPart(Socket socket) {
            var buffer = new byte[1024];
            socket.Receive(buffer);

            var serverResponse = new BinaryReader(new MemoryStream(buffer));

            // check for unencoded data
            if (serverResponse.ReadByte() != 0xFF) {
                serverResponse.BaseStream.Seek(-1, SeekOrigin.Current);
            }

            if (serverResponse.ReadInt32() != (int) MasterResponseCommands.BeginServerListPart) {
                throw new ServerListFetcherException("Incorrect response");
            }

            // read unused packet number
            serverResponse.ReadByte();

            if (serverResponse.ReadByte() != (int) MasterResponseCommands.ServerBlock) {
                throw new ServerListFetcherException("Incorrect response");
            }

            return serverResponse;
        }

        private static IEnumerable<IPEndPoint> ReadServers(BinaryReader serverResponse) {
            // we read 1 byte so we need to seek 1 byte backward
            serverResponse.BaseStream.Seek(-1, SeekOrigin.Current);

            var portsAmount = serverResponse.ReadByte();
            var ip = serverResponse.ReadUInt32();

            for (var i = 0; i < portsAmount; i++) {
                yield return new IPEndPoint(ip, serverResponse.ReadUInt16());
            }
        }

        private static ResponseReadStates FindOutIsItEndOfServerList(BinaryReader serverResponse) {
            var responseCommand = (MasterResponseCommands) serverResponse.ReadByte();

            switch (responseCommand) {
                case MasterResponseCommands.EndServerList:
                    return ResponseReadStates.End;
                case MasterResponseCommands.EndServerListPart:
                    return ResponseReadStates.WaitForNextServerListPart;
                default:
                    throw new ServerListFetcherException("Incorrect response");
            }
        }
    }
}