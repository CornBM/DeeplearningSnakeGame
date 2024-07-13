using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SnakeGame.Interface;

namespace SnakeGame.Class
{
    internal class NetworkInterface : FunctionInterface
    {
        private UdpClient udpClient;
        private Thread receiveThread;
        private bool isReceiving;

        private DataInterface Data;

        public NetworkInterface(DataInterface data, int localPort)
        {
            udpClient = new UdpClient(localPort);
            this.Data = data;
        }

        public void Start()
        {
            isReceiving = true;
            receiveThread = new Thread(() =>
            {
                try
                {
                    while (isReceiving)
                    {
                        Receive();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error receiving data: " + e.Message);
                }
            });
            receiveThread.Start();
        }

        private void Receive()
        {
            // 接收数据
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
            string message = Encoding.ASCII.GetString(receivedBytes);

            Console.WriteLine($"Received from {remoteEndPoint}: {message}");
            ProcessMessage(message, remoteEndPoint);
        }

        private void ProcessMessage(string message, IPEndPoint remoteEndPoint)
        {
            // 处理接收到的消息
            // 这里可以添加你的业务逻辑
        }

        public void Stop()
        {
            isReceiving = false;
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Abort();
            }
            udpClient.Close();
        }
    }
}
