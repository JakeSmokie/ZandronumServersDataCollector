using System.Net.Sockets;
using System.Threading.Tasks;

namespace ZandronumServersDataCollector.Extensions {
    public static class UdpClientExtension {
        public static async Task<byte[]> ReceiveDataWithTimeout(this UdpClient udpClient, int timeout) {
            var receiveTask = udpClient.ReceiveAsync();
            await Task.Delay(timeout);

            if (receiveTask.Status == TaskStatus.WaitingForActivation)
                return null;

            return receiveTask.Result.Buffer;
        }
    }
}