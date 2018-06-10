using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using ZSharedLib.Structures;

namespace ZSharedLib.DBDrivers.PostgreSQLDBDriver {
    public sealed class DBServer {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public DateTime LogTime { get; set; }

        [Required]
        public IPAddress Address { get; set; }

        [Required]
        public ushort Port { get; set; }

        [Required]
        public short Ping { get; set; }

        [Required]
        public string Version { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Map { get; set; }

        [Required]
        public byte MaxClients { get; set; }

        [Required]
        public bool ForcePassword { get; set; }

        [Required]
        public byte NumPlayers { get; set; }

        [Required]
        public string Iwad { get; set; }

        [Required]
        public byte Skill { get; set; }

        [Required]
        public GameTypes GameType { get; set; }

        public List<DBFlag> Flags { get; set; }

        public List<DBPlayer> Players { get; set; }

        public List<DBPWad> PWads { get; set; }

        public DBServer() {
        }

        public DBServer(ServerData serverData) {
            Address = serverData.Address.Address;
            Port = (ushort) serverData.Address.Port;
            PWads = serverData.PWads.Select(x => new DBPWad {Name = x}).ToList();
            Version = serverData.Version;
            Name = serverData.Name;
            Players = serverData.Players.Select(x => new DBPlayer(x)).ToList();
            Flags = serverData.Flags.Select((x, i) => new DBFlag {Value = x, Type = (byte) i}).ToList();
            GameType = serverData.GameType;
            Skill = serverData.Skill;
            Iwad = serverData.Iwad;
            NumPlayers = serverData.NumPlayers;
            ForcePassword = serverData.ForcePassword;
            MaxClients = serverData.MaxClients;
            Ping = serverData.Ping;
            LogTime = serverData.LogTime;
            Map = serverData.Map;
        }
    }
}