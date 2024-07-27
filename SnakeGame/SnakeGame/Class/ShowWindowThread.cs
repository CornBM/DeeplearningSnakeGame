using SnakeGame.Gui;
using SnakeGame.Interface;
using System;
using System.Threading;
using System.Windows.Forms;

namespace SnakeGame.Class
{
    public class ShowWindowThread : FunctionInterface
    {
        private bool isRunning = false;
        private Thread window;
        private ShowWindow S;

        public ShowWindowThread(ShowWindow s)
        {
            S = s;
        }
        public bool IsRunning()
        {
            return isRunning;
        }

        public void Start()
        {
            isRunning = true;
            window = new Thread(() =>
            {
                Application.Run(S);
                isRunning = false;
            });
            window.Start();
        }

        public void Stop()
        {
            isRunning = false;
            window.Abort();
        }
    }
}
