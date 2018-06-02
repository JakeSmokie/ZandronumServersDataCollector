using System.Collections.Generic;
using ZSharedLib.Structures;

namespace ZSharedLib.DBDrivers {
    public interface IDbDriver {
        void Connect();
        void InsertServerData(ServerData serverData);
        IEnumerable<ServerData> SelectServerData();
    }
}