using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using EncodeLibrary.Huffman;
using ZandronumServersDataCollector.BinaryReaderExtensions;

namespace ZandronumServersDataCollector.ServerDataFetchers {
    public partial class ServerDataFetcher : IServerDataFetcher {
        private static readonly HuffmanCodec HuffmanCodec =
            new HuffmanCodec(HuffmanCodec.SkulltagCompatibleHuffmanTree);

        public IEnumerable<ServerData> FetchServerData(IEnumerable<IPEndPoint> servers) {
            return servers.Select(FetchServerData);
        }

        public ServerData FetchServerData(IPEndPoint server) {
            using (var udpClient = new UdpClient()) {
                ConnectAndSendQuery(server, udpClient);
                var data = RecievePlainData(udpClient);

                return ReadResponse(data, server);
            }
        }

        private static ServerData ReadResponse(byte[] data, IPEndPoint server) {
            var response = new BinaryReader(new MemoryStream(data));
            var responseType = (ResponseTypes) response.ReadInt32();

            switch (responseType) {
                case ResponseTypes.Good:
                    break;
                case ResponseTypes.Wait:
                    return null;
                case ResponseTypes.Banned:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var responseTimestamp = response.ReadUInt32();
            var serverData =
                new ServerData {
                    Address = server,
                    Ping = (uint) DateTime.Now.TimeOfDay.TotalMilliseconds - responseTimestamp,
                    Version = response.ReadNullTerminatedString()
                };

            var flags = (ZandronumQueryFlags) response.ReadInt32();

            if (flags.HasFlag(ZandronumQueryFlags.Name)) {
                serverData.Name = response.ReadNullTerminatedString();
            }

            return serverData;
        }

        private static byte[] RecievePlainData(UdpClient udpClient) {
            var recieveBuffer = new List<byte>();

            do {
                IPEndPoint ip = null;
                recieveBuffer.AddRange(udpClient.Receive(ref ip));
            } while (udpClient.Available != 0);

            return HuffmanCodec.Decode(recieveBuffer.ToArray());
        }

        private static void ConnectAndSendQuery(IPEndPoint server, UdpClient udpClient) {
            var query = ConstructServerQuery();
            udpClient.Connect(server);

            var sendTask = udpClient.SendAsync(query, query.Length);
            sendTask.Wait();
        }

        private static byte[] ConstructServerQuery() {
            var query = new List<byte>();

            // send server launcher challenge
            query.AddRange(BitConverter.GetBytes(199));
            query.AddRange(BitConverter.GetBytes((int) ZandronumQueryFlags.Name));
            query.AddRange(BitConverter.GetBytes((int) DateTime.Now.TimeOfDay.TotalMilliseconds));

            return HuffmanCodec.Encode(query.ToArray());
        }
    }
}
