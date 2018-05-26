namespace ZandronumServersDataCollector
{
    public enum MasterResponseCommands : int {
        BeginServerList,
        Server,
        EndServerList,
        IpIsBanned,
        RequestIgnored,
        WrongVersion,
        BeginServerListPart,
        EndServerListPart,
        ServerBlock,
    };
}