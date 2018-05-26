using System.Collections.Generic;
using System.Linq;
using System.Net;
using ZandronumServersDataCollector.ServerDataFetchers;
using ZandronumServersDataCollector.ServerListFetchers;

namespace ZandronumServersDataCollector {
    internal class Program {
        private static void Main(string[] args) {
            var serverListFetcher = new ServerListFetcher();
            var serverDataFetcher = new ServerDataFetcher();

            var serverAddresses = serverListFetcher.FetchServerList(("master.zandronum.com", 15300)).ToList();
            var servers = new List<ServerData>();

            serverDataFetcher.FetchServerData(serverAddresses, servers);
        }
    }
}