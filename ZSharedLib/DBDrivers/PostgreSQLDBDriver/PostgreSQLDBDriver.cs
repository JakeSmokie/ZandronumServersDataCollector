using System;
using System.Collections.Generic;
using System.Linq;
using ZSharedLib.Structures;

namespace ZSharedLib.DBDrivers.PostgreSQLDBDriver {
    public sealed class PostgreSQLDBDriver : IDBDriver {
        private ServersContext _db;

        public void Connect() {
            _db = new ServersContext();
        }

        public async void InsertServerData(IEnumerable<ServerData> serverDatas) {
            Dispose();
            Connect();

            await _db.ServerLogs.AddRangeAsync(serverDatas.Select(x => new DBServer(x)));
            await _db.SaveChangesAsync();
        }

        public IEnumerable<ServerData> SelectServerData() {
            throw new NotImplementedException();
        }

        public void Dispose() {
            _db?.Dispose();
        }
    }
}