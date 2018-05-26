using System.Collections.Generic;
using System.Net;

namespace ZandronumServersDataCollector.ServerDataFetchers {
    public interface IServerDataFetcher {
        IEnumerable<ServerData> FetchServerData(IEnumerable<IPEndPoint> servers);
        ServerData FetchServerData(IPEndPoint server);
    }
}