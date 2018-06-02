using System.Collections.Generic;
using System.Net;
using ZSharedLib.Structures;

namespace ZandronumServersDataCollector.ServerDataFetchers {
    public interface IServerDataFetcher {
        void FetchServerData(IEnumerable<IPEndPoint> servers, List<ServerData> serverDatasCollection);
        void FetchServerData(IPEndPoint server, List<ServerData> serverDatasCollection);
    }
}