using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
            using (var udpClient = new UdpClient()) {
                ConnectAndSendQuery(masterServer, udpClient);

                var receiveTask = ReceivePlainData(udpClient);
                receiveTask.Wait();

                var recievedData = receiveTask.Result;

                if (recievedData == null)
                    return new List<IPEndPoint>();

                // check for unencoded data
                var serverResponse =
                    recievedData[0] == 0xFF
                        ? recievedData.Skip(1).ToArray()
                        : HuffmanCodec.Decode(recievedData.ToArray());

                return ReadResponse(new BinaryReader(new MemoryStream(serverResponse))).ToArray();
            }
        }

        private static void ConnectAndSendQuery((string host, int port) masterServer, UdpClient udpClient) {
            udpClient.Connect(masterServer.host, masterServer.port);
            var query = ConstructLauncherQuery();

            var sendTask = udpClient.SendAsync(query, query.Length);
            sendTask.Wait();
        }

        private static byte[] ConstructLauncherQuery() {
            var message = new List<byte>();

            message.AddRange(BitConverter.GetBytes((int) MasterServerCommands.LauncherMasterChallenge));
            message.AddRange(BitConverter.GetBytes((short) MasterServerVersion.UsualVersion));

            return HuffmanCodec.Encode(message.ToArray());
        }

        private static async Task<byte[]> ReceivePlainData(UdpClient udpClient) {
            var recievedData = new List<byte>();

            do {
                var data = await udpClient.ReceiveWithTimeoutAndAmountOfAttempts(Timeout, AttemptsAmount);

                if (data == null)
                    return null;

                recievedData.AddRange(data);
            } while (udpClient.Available != 0);

            return recievedData.ToArray();
        }

        private static IEnumerable<IPEndPoint> ReadResponse(BinaryReader serverResponse) {
            var response = (MasterResponseCommands) serverResponse.ReadInt32();

            if (response == MasterResponseCommands.IpIsBanned ||
                response == MasterResponseCommands.RequestIgnored) {
                yield break;
            }

            serverResponse.BaseStream.Seek(-4, SeekOrigin.Current);
            var state = ResponseReadStates.WaitForNextServerListPart;

            while (state != ResponseReadStates.End) {
                switch (state) {
                    case ResponseReadStates.WaitForNextServerListPart:
                        WaitForNextServerListPart(serverResponse);
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

        private static void WaitForNextServerListPart(BinaryReader serverResponse) {
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
        }
    }
}