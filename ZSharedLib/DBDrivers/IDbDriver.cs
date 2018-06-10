using System;
using System.Collections.Generic;
using ZSharedLib.Structures;

namespace ZSharedLib.DBDrivers {
    public interface IDBDriver : IDisposable {
        void Connect();
        IEnumerable<ServerData> SelectServerData();
        void InsertServerData(IEnumerable<ServerData> serverDatas);
    }
}