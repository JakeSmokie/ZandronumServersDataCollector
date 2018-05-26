using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using EncodeLibrary.Huffman;
using ZandronumServersDataCollector.BinaryReaderExtensions;

namespace ZandronumServersDataCollector.ServerDataFetchers {
    public partial class ServerDataFetcher {
        private static readonly HuffmanCodec HuffmanCodec =
            new HuffmanCodec(HuffmanCodec.SkulltagCompatibleHuffmanTree);

        public void FetchServerData(IEnumerable<IPEndPoint> servers, List<ServerData> serverDatasCollection) {
            var tasks = new List<Task>();

            foreach (var address in servers) {
                var task = FetchServerData(address, serverDatasCollection);
                tasks.Add(task);
            }

            foreach (var task in tasks) {
                task.Wait();
            }
        }

        public async Task FetchServerData(IPEndPoint server, List<ServerData> serverDatasCollection) {
            Console.WriteLine(server);

            using (var udpClient = new UdpClient()) {
                await ConnectAndSendQuery(server, udpClient);
                var data = await RecievePlainData(udpClient);

                await ReadResponse(data, server, serverDatasCollection);
            }
        }

        private static async Task ConnectAndSendQuery(IPEndPoint server, UdpClient udpClient) {
            var query = ConstructServerQuery();
            udpClient.Connect(server);

            await udpClient.SendAsync(query, query.Length);
        }

        private static byte[] ConstructServerQuery() {
            var query = new List<byte>();

            // send server launcher challenge
            query.AddRange(BitConverter.GetBytes(199));
            query.AddRange(BitConverter.GetBytes((int) ZandronumQueryFlags.Name));
            query.AddRange(BitConverter.GetBytes((int) DateTime.Now.TimeOfDay.TotalMilliseconds));

            return HuffmanCodec.Encode(query.ToArray());
        }

        private static async Task<byte[]> RecievePlainData(UdpClient udpClient) {
            var data = await udpClient.ReceiveAsync();
            return HuffmanCodec.Decode(data.Buffer);
        }

        private static Task ReadResponse(byte[] data, IPEndPoint server, List<ServerData> serverDatasCollection) {
            var response = new BinaryReader(new MemoryStream(data));
            var responseType = (ResponseTypes) response.ReadInt32();

            switch (responseType) {
                case ResponseTypes.Good:
                    break;
                case ResponseTypes.Wait:
                    return Task.CompletedTask;
                case ResponseTypes.Banned:
                    return Task.CompletedTask;
                default:
                    return Task.CompletedTask;
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

            serverDatasCollection.Add(serverData);
            return Task.CompletedTask;
        }
    }
}