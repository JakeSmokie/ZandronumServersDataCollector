using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using EncodeLibrary.Huffman;
using ZandronumServersDataCollector.Extensions;

namespace ZandronumServersDataCollector.ServerDataFetchers {
    public partial class ServerDataFetcher {
        /// <summary>
        /// Timeout for connection to server in milliseconds
        /// </summary>
        private const int Timeout = 1000;

        /// <summary>
        /// Amount of server connection attempts
        /// </summary>
        private const int ServerConnectionAttemptsAmount = 5;

        private static readonly HuffmanCodec HuffmanCodec =
            new HuffmanCodec(HuffmanCodec.SkulltagCompatibleHuffmanTree);

        public async Task FetchServerData(IEnumerable<IPEndPoint> servers, List<ServerData> serverDatasCollection) {
            await Task.Delay(1);

            foreach (var address in servers) {
                FetchServerData(address, serverDatasCollection);
                await Task.Delay(10);
            }

            Console.WriteLine($"Got {serverDatasCollection.Count} server.");
        }

        public async Task FetchServerData(IPEndPoint server, List<ServerData> serverDatasCollection) {
            await Task.Delay(1);

            using (var udpClient = new UdpClient()) {
                await ConnectAndSendQuery(server, udpClient);
                var data = await RecievePlainData(udpClient);

                if (data == null) {
                    Console.WriteLine($"Cannot get data from server {server}.");
                    return;
                }

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
            query.AddRange(BitConverter.GetBytes((int) ZandronumQueryFlags.Standardquery));
            query.AddRange(BitConverter.GetBytes((int) DateTime.Now.TimeOfDay.TotalMilliseconds));

            return HuffmanCodec.Encode(query.ToArray());
        }

        private static async Task<byte[]> RecievePlainData(UdpClient udpClient) {
            var data = await udpClient.ReceiveWithTimeoutAndAmountOfAttempts(Timeout, ServerConnectionAttemptsAmount);
            return data == null ? null : HuffmanCodec.Decode(data);
        }

        private static Task ReadResponse(byte[] data, IPEndPoint server, List<ServerData> serverDatasCollection) {
            var response = new BinaryReader(new MemoryStream(data));
            var responseType = (ResponseTypes) response.ReadInt32();

            if (responseType != ResponseTypes.Good)
                return Task.CompletedTask;

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

            if (flags.HasFlag(ZandronumQueryFlags.Pwads)) {
                var pwadsAmount = response.ReadByte();
                serverData.PWads = new List<string>();

                for (var i = 0; i < pwadsAmount; i++) {
                    serverData.PWads.Add(response.ReadNullTerminatedString());
                }
            }

            Console.WriteLine(serverData);

            serverDatasCollection.Add(serverData);
            return Task.CompletedTask;
        }
    }
}