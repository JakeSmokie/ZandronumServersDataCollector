using System.Linq;

namespace ZandronumServersDataCollector {
    internal class Program {
        private static void Main(string[] args) {
            var fetcher = new ServerListFetcher();
            var servers = fetcher.FetchServerList(("master.zandronum.com", 15300)).ToList();


        }
    }
}