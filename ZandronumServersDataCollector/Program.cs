using System.Net;

namespace ZandronumServersDataCollector {
    internal class Program {
        private static void Main(string[] args) {
            var fetcher = new ServerListFetcher();
            var servers = fetcher.FetchServerList(new[] {
                ("master.zandronum.com", 15300),
                ("localhost", 15300)
            });
        }
    }
}