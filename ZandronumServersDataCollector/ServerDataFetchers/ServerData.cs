using System.Collections.Generic;
using System.Net;

namespace ZandronumServersDataCollector.ServerDataFetchers {
    public class ServerData {
        public IPEndPoint Address;
        public uint Ping;
        public string Version;
        public string Name;
        public List<string> PWads;

        public override string ToString() {
            return $"{nameof(Address)}: {Address}, {nameof(Ping)}: {Ping}, {nameof(Version)}: {Version}, {nameof(Name)}: {Name}";
        }
    }
}