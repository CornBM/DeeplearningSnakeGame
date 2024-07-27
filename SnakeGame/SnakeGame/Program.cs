using System;
using System.Threading;
using System.Windows.Forms;

namespace SnakeGame
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        public static Form1 f;
        [STAThread]
        static void Main()
        {
            Thread t = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                f = new Form1();
                Application.Run(f);
            });
            t.Start();
        }
    }
}
