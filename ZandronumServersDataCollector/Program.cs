using System.Collections.Generic;
using System.Linq;
using ZandronumServersDataCollector.ServerDataFetchers;
using ZandronumServersDataCollector.ServerListFetchers;

namespace ZandronumServersDataCollector {
    internal class Program {
        private static void Main(string[] args) {
            var serverListFetcher = new ServerListFetcher();
            var serverDataFetcher = new ServerDataFetcher();

            var serverList = serverListFetcher.FetchServerList(("master.zandronum.com", 15300)).ToList();
            var servers = new List<ServerData>();

            serverDataFetcher.FetchServerData(serverList.Take(1).ToList(), servers);
        }
    }
}