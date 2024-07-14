using SnakeGame.Gui;
using SnakeGame.Interface;
using SnakeGame.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeGame.Class
{
    internal class GameSystem : FunctionInterface
    {
        public int Id;

        private InIFile ini;
        private bool isRunning = false;
        private List<FunctionInterface> funcs;

        public DataInterface Data;
        public GameSystem(string configPath, int id)
        {
            Id = id;

            ini = new InIFile(configPath);
            Random r = new Random();
            funcs = new List<FunctionInterface>();

            DataInterface data = new Data(
                new Map(
                    int.Parse(ini.Read("width", "Map")),
                    int.Parse(ini.Read("height", "Map"))
                    ),
                (Direction)r.Next(0, 4));

            Console.WriteLine($"{data.MapWidth()}, {data.MapHeight()}");
            try
            {
                funcs.Add(new ShowWindow(
                    data,
                    int.Parse(ini.Read("interval", "Show")),
                    int.Parse(ini.Read("length", "Show"))));

                funcs.Add(new SnakeGameI(
                    data,
                    new Snake(data, int.Parse(ini.Read("length", "Snake"))),
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
            foreach(FunctionInterface func in funcs)
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
            Program.f.M.StopGame(Id);
        }

        public bool IsRunning()
        {
            return isRunning;
        }
    }
}
