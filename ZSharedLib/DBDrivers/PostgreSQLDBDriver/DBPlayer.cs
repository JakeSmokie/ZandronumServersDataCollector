using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZSharedLib.Structures;

namespace ZSharedLib.DBDrivers.PostgreSQLDBDriver {
    public sealed class DBPlayer {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public short Score { get; set; }

        [Required]
        public short Ping { get; set; }

        [Required]
        public bool IsSpectator { get; set; }

        [Required]
        public bool IsBot { get; set; }

        [Required]
        public sbyte Team { get; set; }

        [Required]
        public sbyte PlayTime { get; set; }

        [Required]
        public long ServerId { get; set; }

        [Required]
        public DBServer Server { get; set; }

        public DBPlayer() {
        }

        public DBPlayer(Player player) {
            Name = player.Name;
            Score = player.Score;
            Ping = player.Ping;
            IsSpectator = player.IsSpectator;
            IsBot = player.IsBot;
            Team = player.Team;
            PlayTime = player.PlayTime;
        }
    }
}