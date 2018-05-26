using System;

namespace ZandronumServersDataCollector.ServerListFetchers {
    public class ServerListFetcherException : Exception {
        public ServerListFetcherException(string message) : base(message) {
        }
    }
}