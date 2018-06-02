namespace ZSharedLib.Structures {
    public struct Player {
        public string Name { get; set; }
        public short Score { get; set; }
        public short Ping { get; set; }
        public bool IsSpectator { get; set; }
        public bool IsBot { get; set; }
        public sbyte Team { get; set; }
        public sbyte PlayTime { get; set; }
    }
}