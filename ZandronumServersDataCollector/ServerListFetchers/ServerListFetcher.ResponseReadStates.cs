namespace ZandronumServersDataCollector.ServerListFetchers {
    public partial class ServerListFetcher {
        private enum ResponseReadStates {
            WaitForNextServerListPart,
            ReadServerBlock,
            End
        }
    }
}