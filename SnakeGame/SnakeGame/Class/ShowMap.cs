using SnakeGame.Gui;
using SnakeGame.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeGame.Class
{
    internal class ShowMap : FunctionInterface
    {
        private DataInterface Data;
        private Thread showThread;
        private bool isRunning = false;
        private int interval;

        private ShowWindow showWindow;
        public ShowMap(DataInterface data, int interval, int length)
        {
            Data = data;
            this.interval = interval;
            showWindow = new ShowWindow(data, length);
        }
        public void Start()
        {
            isRunning = true;
            showWindow.Visible = true;
            showThread = new Thread(() => {
                while (isRunning)
                {
                    Show();
                    Thread.Sleep(interval);
                }
            });
            showThread.Start();
        }

        public void Show()
        {
             showWindow.ShowMap();

        }

        public void Stop()
        {
            isRunning = false;
            showWindow.Hide();
            if (showThread != null && showThread.IsAlive)
            {
                showThread.Join();
            }
        }
    }
}
