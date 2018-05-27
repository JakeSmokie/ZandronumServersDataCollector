using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using ZandronumServersDataCollector.ServerDataFetchers;
using ZandronumServersDataCollector.ServerListFetchers;

namespace ZandronumServersDataCollector {
    internal class Program {
        private static void Main(string[] args) {
            var serverListFetcher = new ServerListFetcher();
            var serverDataFetcher = new ServerDataFetcher();

            var serverAddresses = serverListFetcher.FetchServerList(("master.zandronum.com", 15300));
            var servers = new List<ServerData>();

            Console.WriteLine($"Got servers list. {serverAddresses?.Count() ?? 0} total");

            var task = serverDataFetcher.FetchServerData(serverAddresses, servers);
            task.Wait();

            Console.WriteLine("Got servers data.");

            while (true) {
                Console.ReadLine();
            }
        }
    }
}