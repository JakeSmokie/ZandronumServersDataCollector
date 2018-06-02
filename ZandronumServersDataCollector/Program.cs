using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ZandronumServersDataCollector.DBDrivers;
using ZandronumServersDataCollector.ServerDataFetchers;
using ZandronumServersDataCollector.ServerListFetchers;

namespace ZandronumServersDataCollector {
    internal class Program {
        private const int ServersUpdateInteval = 5000;

        private static void Main(string[] args) {
            var serverListFetcher = new ServerListFetcher();
            var serverDataFetcher = new ServerDataFetcher();

            var serverDatas = new List<ServerData>();

            var dbDriver = new CassandraDbDriver();
            dbDriver.Connect();

            while (true) {
                var serverAddresses = serverListFetcher.FetchServerList(("master.zandronum.com", 15300)).ToList();
                serverDatas.Clear();

                Console.WriteLine($"Got servers list. {serverAddresses?.Count} total");
                serverDataFetcher.FetchServerData(serverAddresses, serverDatas);
                Console.WriteLine($"Got {serverDatas.Count} servers data.");
            
                foreach (var server in serverDatas) {
                    new Thread(() => { dbDriver.InsertServerData(server); }).Start();
                }

                GC.Collect();
                Thread.Sleep(ServersUpdateInteval);
            }
        }
    }
}