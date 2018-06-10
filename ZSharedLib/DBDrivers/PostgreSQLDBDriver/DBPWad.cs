using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZSharedLib.DBDrivers.PostgreSQLDBDriver {
    public sealed class DBPWad {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public long ServerId { get; set; }

        [Required]
        public DBServer Server { get; set; }
    }
}