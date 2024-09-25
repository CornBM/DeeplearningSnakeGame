using SnakeGame.Gui;
using SnakeGame.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame.Class
{
    public class ShowWindowThread : FunctionInterface
    {
        private bool isRunning = false;
        private Task window;
        private ShowWindow S;

        public ShowWindowThread(ShowWindow s)
        {
            S = s;
        }
        public bool IsRunning()
        {
            return isRunning;
        }

        public async Task Start()
        {
            isRunning = true;
            window = Task.Run(() =>
            {
                Application.Run(S);
            });
            await window;
            isRunning = false;
        }

        public void Stop()
        {
            S.Close();
        }
    }
}
