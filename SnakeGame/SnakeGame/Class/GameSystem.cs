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
        private InIFile ini;
        private bool isRunning = false;
        private List<FunctionInterface> funcs;

        private DataInterface Data;
        public GameSystem(string configPath)
        {
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
                funcs.Add(new ShowMap(
                    data,
                    int.Parse(ini.Read("interval", "Show")),
                    int.Parse(ini.Read("length", "Show"))));


                funcs.Add(new NetworkInterface(
                    data,
                    int.Parse(ini.Read("port", "Network"))));

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
        }

        public void Stop()
        {
            isRunning = false;
            foreach (FunctionInterface func in funcs)
            {
                func.Stop();
            }
        }
    }
}
