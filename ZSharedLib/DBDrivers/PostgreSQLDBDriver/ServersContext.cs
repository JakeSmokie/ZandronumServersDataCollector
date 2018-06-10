using Microsoft.EntityFrameworkCore;

namespace ZSharedLib.DBDrivers.PostgreSQLDBDriver {
    public sealed class ServersContext : DbContext {
        public DbSet<DBServer> ServerLogs { get; set; }

        public ServersContext() {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=123");
        }
    }
}