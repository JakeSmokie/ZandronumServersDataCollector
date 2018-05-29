using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
        private const int ServerConnectionAttemptsAmount = 3;

        private const int MaximumTaskAmount = 16;

        private static readonly HuffmanCodec HuffmanCodec =
            new HuffmanCodec(HuffmanCodec.SkulltagCompatibleHuffmanTree);

        private int _lastUsedPort = 40000;

        private static readonly Semaphore Semaphore = new Semaphore(MaximumTaskAmount, MaximumTaskAmount);
        private readonly object _lockObject = new object();

        public void FetchServerData(IEnumerable<IPEndPoint> servers, List<ServerData> serverDatasCollection) {
            var threads = new List<Thread>();

            foreach (var server in servers) {
                var t = new Thread(() => { FetchServerData(server, serverDatasCollection); });
                threads.Add(t);
            }

            foreach (var t in threads) {
                t.Start();
            }

            foreach (var t in threads) {
                t.Join();
            }
        }

        public void FetchServerData(IPEndPoint server, List<ServerData> serverDatasCollection) {
            byte[] data;

            using (var socket = new Socket(SocketType.Dgram, ProtocolType.Udp) {
                ReceiveTimeout = Timeout
            }) {
                lock (_lockObject) {
                    socket.Bind(new IPEndPoint(IPAddress.Any, _lastUsedPort++));
                }

                data = RecievePlainData(server, socket);

                if (data == null) {
                    return;
                }
            }

            ReadResponse(data, server, serverDatasCollection);
        }

        private static void ConnectAndSendQuery(IPEndPoint server, Socket socket) {
            socket.Connect(server);
            socket.Send(ConstructServerQuery());
        }

        private static byte[] ConstructServerQuery() {
            // not so beauty, but fast
            var query = new byte[12];
            query[0] = 199;

            var flags = BitConverter.GetBytes((int) ZandronumQueryFlags.Standardquery);

            for (var index = 0; index < flags.Length; index++) {
                query[4 + index] = flags[index];
            }

            var time = BitConverter.GetBytes((int) DateTime.Now.TimeOfDay.TotalMilliseconds);

            for (var index = 0; index < time.Length; index++) {
                query[8 + index] = time[index];
            }

            return HuffmanCodec.Encode(query);
        }

        private static byte[] RecievePlainData(IPEndPoint server, Socket socket) {
            var buffer = new byte[4096];
            var recievedAmount = 0;

            for (var i = 0; i < ServerConnectionAttemptsAmount; i++) {
                ConnectAndSendQuery(server, socket);

                try {
                    recievedAmount = socket.Receive(buffer);
                }
                catch (SocketException) {
                    continue;
                }

                break;
            }

            if (recievedAmount == 0) {
                //Console.WriteLine($"Can't connect to server {socket.RemoteEndPoint as IPEndPoint}");
                return null;
            }

            var recievedData = buffer.Take(recievedAmount).ToArray();

            return recievedData[0] == 0xFF
                ? recievedData.Skip(1).ToArray()
                : HuffmanCodec.Decode(recievedData.ToArray());
        }

        private static void ReadResponse(byte[] data, IPEndPoint server,
            ICollection<ServerData> serverDatasCollection) {
            var response = new BinaryReader(new MemoryStream(data));
            var responseType = (ResponseTypes) response.ReadInt32();

            if (responseType != ResponseTypes.Good) return;

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
        }
    }
}