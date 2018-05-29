using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ZandronumServersDataCollector.DBDrivers;
using ZandronumServersDataCollector.ServerDataFetchers;
using ZandronumServersDataCollector.ServerListFetchers;

namespace ZandronumServersDataCollector {
    internal class Program {
        private static void Main(string[] args) {
            while (true) {
                var serverListFetcher = new ServerListFetcher();
                var serverDataFetcher = new ServerDataFetcher();

                var serverAddresses = serverListFetcher.FetchServerList(("master.zandronum.com", 15300)).ToList();
                var serverDatas = new List<ServerData>();

                Console.WriteLine($"Got servers list. {serverAddresses?.Count} total");
                serverDataFetcher.FetchServerData(serverAddresses, serverDatas);
                Console.WriteLine($"Got {serverDatas.Count} servers data.");

                var dbDriver = new DbDriver();
                dbDriver.Connect();

                foreach (var server in serverDatas) {
                    dbDriver.InsertServerData(server);
                }

                GC.Collect();
            }
        }
    }
}