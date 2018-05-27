using System.Net.Sockets;
using System.Threading.Tasks;

namespace ZandronumServersDataCollector.Extensions {
    public static class UdpClientExtension {
        public static byte[] ReceiveWithTimeout(this UdpClient udpClient, int timeout) {
            var receiveTask = udpClient.ReceiveAsync();
            receiveTask.Wait(timeout);

            return receiveTask.Status == TaskStatus.WaitingForActivation ? null : receiveTask.Result.Buffer;
        }

        public static byte[] ReceiveWithTimeoutAndAmountOfAttempts(this UdpClient udpClient, int timeout, int attemptsAmount) {
            byte[] data = null;

            for (var i = 0; i < attemptsAmount; i++) {
                data = udpClient.ReceiveWithTimeout(timeout);

                if (data != null)
                    break;
            }

            return data;
        }
    }
}