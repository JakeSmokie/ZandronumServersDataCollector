namespace ZandronumServersDataCollector.ServerDataFetchers {
    public partial class ServerDataFetcher {
        private enum ZandronumQueryFlags {
            Name = 0x00000001,
            Url = 0x00000002,
            Email = 0x00000004,
            Mapname = 0x00000008,
            Maxclients = 0x00000010,
            Maxplayers = 0x00000020,
            Pwads = 0x00000040,
            Gametype = 0x00000080,
            Gamename = 0x00000100,
            Iwad = 0x00000200,
            Forcepassword = 0x00000400,
            Forcejoinpassword = 0x00000800,
            Gameskill = 0x00001000,
            Botskill = 0x00002000,
            Dmflags = 0x00004000, // Deprecated
            Limits = 0x00010000,
            Teamdamage = 0x00020000,
            Teamscores = 0x00040000,
            Numplayers = 0x00080000,
            Playerdata = 0x00100000,
            TeaminfoNumber = 0x00200000,
            TeaminfoName = 0x00400000,
            TeaminfoColor = 0x00800000,
            TeaminfoScore = 0x01000000,
            TestingServer = 0x02000000,
            DataMd5Sum = 0x04000000,
            AllDmflags = 0x08000000,
            SecuritySettings = 0x10000000,
            OptionalWads = 0x20000000,
            Deh = 0x40000000,

            Standardquery =
                Name | Pwads
        };
    }
}