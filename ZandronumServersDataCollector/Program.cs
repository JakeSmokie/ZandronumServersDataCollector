using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using ZandronumServersDataCollector.ServerDataFetchers;
using ZandronumServersDataCollector.ServerListFetchers;
using ZSharedLib.DBDrivers;
using ZSharedLib.DBDrivers.PostgreSQLDBDriver;
using ZSharedLib.Structures;

namespace ZandronumServersDataCollector {
    internal sealed class Program {
        private static IServerListFetcher _serverListFetcher;
        private static IServerDataFetcher _serverDataFetcher;
        private static IDBDriver _dbDriver;
        private static List<ServerData> _serverDatas;
        private static List<IPEndPoint> _serverAddresses;

        private const int ServersUpdateInteval = 5000;

        private static void Main(string[] args) {
            _serverListFetcher = new ServerListFetcher();
            _serverDataFetcher = new ServerDataFetcher();

            _serverDatas = new List<ServerData>();

            _dbDriver = new PostgreSQLDBDriver();
            _dbDriver.Connect();

            while (true) {
                _serverAddresses = _serverListFetcher.FetchServerList(("master.zandronum.com", 15300)).ToList();
                _serverDatas.Clear();

                Console.WriteLine($"Got servers list. {_serverAddresses.Count} total");
                _serverDataFetcher.FetchServerData(_serverAddresses, _serverDatas);
                Console.WriteLine($"Got {_serverDatas.Count} servers data.");

                _dbDriver.InsertServerData(_serverDatas);

                GC.Collect();
                Thread.Sleep(ServersUpdateInteval);
            }
        }
    }
}