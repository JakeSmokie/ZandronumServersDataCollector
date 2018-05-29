using System.Collections.Generic;
using ZandronumServersDataCollector.ServerDataFetchers;

namespace ZandronumServersDataCollector.DBDrivers {
    public interface IDbDriver {
        void Connect();
        void InsertServerData(ServerData serverData);
        IEnumerable<ServerData> SelectServerData();
    }
}