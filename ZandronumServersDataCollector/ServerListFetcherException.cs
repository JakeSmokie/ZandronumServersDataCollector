using System;

namespace ZandronumServersDataCollector {
    public class ServerListFetcherException : Exception {
        public ServerListFetcherException(string message) : base(message) {
        }
    }
}