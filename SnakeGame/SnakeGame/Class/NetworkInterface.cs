using SnakeGame.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeGame.Class
{
    public class NetworkInterface : FunctionInterface
    {
        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private Thread acceptThread;
        private bool isReceiving;

        private Action<string> process;

        public NetworkInterface(Action<string> process, int localPort)
        {
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), localPort);
            this.process = process;
        }

        public void Start()
        {
            isReceiving = true;
            tcpListener.Start();
            acceptThread = new Thread(() =>
            {
                try
                {
                    while (isReceiving)
                    {
                        Accept();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error receiving data: " + e.Message);
                }
            });
            acceptThread.Start();
        }

        private void Accept()
        {
            // 接受客户端连接
            tcpClient = tcpListener.AcceptTcpClient();
            Thread receiveThread = new Thread(() =>
            {
                HandleClient(tcpClient.GetStream());
            });
            receiveThread.Start();
        }

        private void HandleClient(NetworkStream stream)
        {
            // 接收数据
            byte[] buffer = new byte[1024];
            while (isReceiving)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received: {message}");
                        ProcessMessage(message);
                    }
                }
                catch
                {
                    break;
                }
            }
            // 清理资源
            if (stream != null)
            {
                stream.Close();
            }
            if (tcpClient != null)
            {
                tcpClient.Close();
            }
        }

        public void Send(string message)
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                // 将字符串消息编码为字节数组
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);

                // 使用TcpClient发送字节数组
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
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
            if (acceptThread != null && acceptThread.IsAlive)
            {
                acceptThread.Interrupt();
            }
            if (tcpClient != null && tcpClient.Connected)
            {
                tcpClient.Close();
            }
            tcpListener.Stop();
        }

        public bool IsRunning()
        {
            return isReceiving;
        }
    }
}