using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
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
            for (var i = 0; i < 5; i++) {
                try {
                    await _db.SaveChangesAsync();
                    break;
                }
                catch (NpgsqlOperationInProgressException e) {
                    Console.WriteLine($"[{DateTime.Now}] Exception handled: {e.Message}. Trying again...");
                }
            }
        }

        public IEnumerable<ServerData> SelectServerData() {
            throw new NotImplementedException();
        }

        public void Dispose() {
            _db?.Dispose();
        }
    }
}