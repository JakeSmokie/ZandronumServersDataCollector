using System.Net;

namespace ZandronumServersDataCollector.ServerDataFetchers {
    public class ServerData {
        public IPEndPoint Address;
        public uint Ping;
        public string Version;
        public string Name;
    }
}