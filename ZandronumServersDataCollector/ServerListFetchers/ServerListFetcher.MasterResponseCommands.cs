namespace ZandronumServersDataCollector.ServerListFetchers {
    public partial class ServerListFetcher {
        private enum MasterResponseCommands : int {
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
}