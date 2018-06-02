using System.Collections.Generic;
using System.Net;

namespace ZandronumServersDataCollector.ServerListFetchers {
    public interface IServerListFetcher {
        IEnumerable<IPEndPoint> FetchServerList(IEnumerable<(string host, int port)> masterServers);
        IEnumerable<IPEndPoint> FetchServerList((string host, int port) masterServer);
    }
}