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
    public class NetworkInterface : FunctionInterface
    {
        private UdpClient udpClient;
        private Thread receiveThread;
        private bool isReceiving;

        private Action<string> process;
        private IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        public NetworkInterface(Action<string> process, int localPort)
        {
            udpClient = new UdpClient(localPort);
            this.process = process;
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
            byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
            string message = Encoding.ASCII.GetString(receivedBytes);

            Console.WriteLine($"Received from {remoteEndPoint}: {message}");
            ProcessMessage(message);
        }

        public void Send(string message)
        {
            // 将字符串消息编码为字节数组
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);

            // 使用UdpClient发送字节数组到指定的远程端点
            udpClient.Send(messageBytes, messageBytes.Length, remoteEndPoint);
        }
    

        private void ProcessMessage(string message)
        {
            // 处理接收到的消息
            // 这里可以添加你的业务逻辑
            process(message);
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

        public bool IsRunning()
        {
            return isReceiving;
        }
    }
}
