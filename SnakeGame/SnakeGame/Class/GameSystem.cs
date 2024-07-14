using SnakeGame.Gui;
using SnakeGame.Interface;
using SnakeGame.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame.Class
{
    internal class GameSystem : FunctionInterface
    {
        public int Id;
        private string configPath;

        private InIFile ini;
        private bool isRunning = false;
        private List<FunctionInterface> funcs;

        public Data Data;
        public GameSystem(string configPath, int id)
        {
            Id = id;
            this.configPath = configPath;
            Init();
        }

        public void Init()
        {
            ini = new InIFile(configPath);
            Random r = new Random();
            funcs = new List<FunctionInterface>();

            Data = new Data(
                new Map(
                    int.Parse(ini.Read("width", "Map")),
                    int.Parse(ini.Read("height", "Map"))
                    ),
                (Direction)r.Next(0, 4));

            try
            {
                funcs.Add(new ShowWindowThread(
                    new ShowWindow(
                    Data,
                    int.Parse(ini.Read("interval", "Show")),
                    int.Parse(ini.Read("length", "Show")))));

                funcs.Add(new SnakeGameI(
                    Data,
                    new Snake(Data, int.Parse(ini.Read("length", "Snake"))),
                    int.Parse(ini.Read("interval", "SnakeGame"))
                    ));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void Start()
        {
            isRunning = true;
            foreach (FunctionInterface func in funcs)
            {
                func.Start();
            }

            Thread suicide = new Thread(() =>
            {
                bool ready2die = false;
                while (!ready2die)
                {
                    foreach (FunctionInterface func in funcs)
                    {
                        if (!func.IsRunning())
                        {
                            ready2die = true;
                            break;
                        }
                    }
                    Thread.Sleep(100);
                }
                Stop();
            });
            suicide.Start();

        }

        public void Stop()
        {
            isRunning = false;
            foreach (FunctionInterface func in funcs)
            {
                if(func.IsRunning())
                    func.Stop();
            }
        }

        public bool IsRunning()
        {
            return isRunning;
        }
    }
}
