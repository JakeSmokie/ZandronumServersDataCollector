using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ZandronumServersDataCollector.Extensions {
    public static class UdpClientExtension {
        public static async Task<byte[]> ReceiveWithTimeout(this UdpClient udpClient, int timeout) {
            var task = udpClient.ReceiveAsync();

            using (var timeoutCancellationTokenSource = new CancellationTokenSource()) {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask != task)
                    return null;

                timeoutCancellationTokenSource.Cancel();

                var data = await task;
                return data.Buffer;
            }
        }

        public static async Task<byte[]> ReceiveWithTimeoutAndAmountOfAttempts(this UdpClient udpClient, int timeout,
            int attemptsAmount) {
            byte[] data = null;

            for (var i = 0; i < attemptsAmount; i++) {
                data = await udpClient.ReceiveWithTimeout(timeout);

                if (data != null)
                    break;
            }

            return data;
        }
    }
}