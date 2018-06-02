using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ZandronumServersDataCollector.ServerDataFetchers;
using ZandronumServersDataCollector.ServerListFetchers;
using ZSharedLib.DBDrivers;
using ZSharedLib.Structures;

namespace ZandronumServersDataCollector {
    internal sealed class Program {
        private static IServerListFetcher _serverListFetcher;
        private static IServerDataFetcher _serverDataFetcher;
        private static IDbDriver _dbDriver;
        private static List<ServerData> _serverDatas;

        private const int ServersUpdateInteval = 5000;

        private static void Main(string[] args) {
            _serverListFetcher = new ServerListFetcher();
            _serverDataFetcher = new ServerDataFetcher();

            _serverDatas = new List<ServerData>();

            _dbDriver = new CassandraDbDriver();
            _dbDriver.Connect();

            while (true) {
                var serverAddresses = _serverListFetcher.FetchServerList(("master.zandronum.com", 15300)).ToList();
                _serverDatas.Clear();

                Console.WriteLine($"Got servers list. {serverAddresses?.Count} total");
                _serverDataFetcher.FetchServerData(serverAddresses, _serverDatas);
                Console.WriteLine($"Got {_serverDatas.Count} servers data.");
            
                foreach (var server in _serverDatas) {
                    new Thread(() => { _dbDriver.InsertServerData(server); }).Start();
                }

                GC.Collect();
                Thread.Sleep(ServersUpdateInteval);
            }
        }
    }
}