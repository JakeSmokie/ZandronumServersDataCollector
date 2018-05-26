using System.Linq;
using ZandronumServersDataCollector.ServerDataFetchers;
using ZandronumServersDataCollector.ServerListFetchers;

namespace ZandronumServersDataCollector {
    internal class Program {
        private static void Main(string[] args) {
            var serverListFetcher = new ServerListFetcher();
            var serverDataFetcher = new ServerDataFetcher();

            var servers = serverListFetcher.FetchServerList(("master.zandronum.com", 15300)).ToList();
            var serverData = serverDataFetcher.FetchServerData(servers).ToList();
        }
    }
}