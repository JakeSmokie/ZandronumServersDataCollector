namespace ZandronumServersDataCollector.ServerListFetchers {
    public partial class ServerListFetcher {
        private enum ResponseReadStates {
            GetNextServerListPart,
            ReadServerBlock,
            End,
            WaitForNextServerListPart
        }
    }
}