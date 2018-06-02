using System;
using System.Collections.Generic;
using System.Net;

namespace ZandronumServersDataCollector.ServerDataFetchers {
    public class ServerData {
        public DateTime LogTime;
        public IPEndPoint Address;
        public short Ping;
        public string Version;
        public string Name;
        public List<string> PWads;
        public string Map;
        public byte MaxClients;
        public bool ForcePassword;
        public byte NumPlayers;
        public Player[] Players;
        public int[] Flags;
        public string Iwad;
        public byte Skill;
        public GameTypes GameType;

        public override string ToString() {
            return $"{nameof(Address)}: {Address}, {nameof(Ping)}: {Ping}, {nameof(Version)}: {Version}, {nameof(Name)}: {Name}";
        }
    }
}