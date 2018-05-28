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

        private static readonly object LockObject = new object();

        private int _lastUsedPort = 40000;

        public void FetchServerData(IEnumerable<IPEndPoint> servers, List<ServerData> serverDatasCollection) {
            var tasks = new List<Task>();

            Parallel.ForEach(servers, s => {
                var task = FetchServerData(s, serverDatasCollection);
                tasks.Add(task);
            });

            foreach (var task in tasks) {
                task?.Wait();
            }
        }

        public async Task FetchServerData(IPEndPoint server, List<ServerData> serverDatasCollection) {
            await Task.Delay(1);
            byte[] data;

            using (var socket = new Socket(SocketType.Dgram, ProtocolType.Udp) {
                ReceiveTimeout = 1000
            }) {
                lock (LockObject) {
                    socket.Bind(new IPEndPoint(IPAddress.Any, _lastUsedPort++)); 
                }

                await ConnectAndSendQuery(server, socket);
                data = RecievePlainData(socket);

                if (data == null) {
                    return;
                }
            }

            await ReadResponse(data, server, serverDatasCollection);
        }

        private static async Task ConnectAndSendQuery(IPEndPoint server, Socket socket) {
            socket.Connect(server);

            var query = ConstructServerQuery();
            await socket.SendAsync(new ArraySegment<byte>(query), SocketFlags.None);
        }

        private static byte[] ConstructServerQuery() {
            var query = new List<byte>();

            // send server launcher challenge
            query.AddRange(BitConverter.GetBytes(199));
            query.AddRange(BitConverter.GetBytes((int) ZandronumQueryFlags.Standardquery));
            query.AddRange(BitConverter.GetBytes((int) DateTime.Now.TimeOfDay.TotalMilliseconds));

            return HuffmanCodec.Encode(query.ToArray());
        }

        private static byte[] RecievePlainData(Socket socket) {
            var recievedData = new List<byte>();
            var buffer = new byte[4096];

            try {
                do {
                    var recievedAmount = socket.Receive(buffer);
                    recievedData.AddRange(buffer.Take(recievedAmount));
                } while (socket.Available > 0);
            }
            catch (SocketException e) {
                Debug.WriteLine(e.Message);
                return null;
            }

            return recievedData[0] == 0xFF
                ? recievedData.Skip(1).ToArray()
                : HuffmanCodec.Decode(recievedData.ToArray());
        }

        private static Task ReadResponse(byte[] data, IPEndPoint server,
            ICollection<ServerData> serverDatasCollection) {
            var response = new BinaryReader(new MemoryStream(data));
            var responseType = (ResponseTypes) response.ReadInt32();

            if (responseType != ResponseTypes.Good)
                return Task.CompletedTask;

            var responseTimestamp = response.ReadUInt32();
            var serverData =
                new ServerData {
                    Address = server,
                    Ping = (short) (DateTime.Now.TimeOfDay.TotalMilliseconds - responseTimestamp),
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

            serverData.LogTime = DateTime.Now;
            serverDatasCollection.Add(serverData);

            return Task.CompletedTask;
        }
    }
}