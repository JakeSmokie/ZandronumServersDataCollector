using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZSharedLib.DBDrivers.PostgreSQLDBDriver {
    public class DBFlag {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public int Value { get; set; }

        [Required]
        public byte Type { get; set; }

        [Required]
        public long ServerId { get; set; }

        [Required]
        public DBServer Server { get; set; }
    }
}