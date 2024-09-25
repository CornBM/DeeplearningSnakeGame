using SnakeGame.Gui;
using SnakeGame.Interface;
using SnakeGame.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeGame.Class
{
    internal class GameSystem : FunctionInterface
    {
        public int Id;  // 当前游戏系统被分配的ID
        private string configPath;

        private InIFile ini;
        private bool isRunning = false;
        private Task _system;
        private List<FunctionInterface> funcs;  // 一个游戏系统包含的功能模块

        public Data Data; // 游戏系统的数据接口
        public GameSystem(string configPath, int id)
        {
            Id = id;
            this.configPath = configPath;
            Init();
        }

        public void Init()
        {
            // 初始化配置文件
            ini = new InIFile(configPath);
            Random r = new Random();
            // 初始化功能模块
            funcs = new List<FunctionInterface>();

            // 初始化数据接口
            Data = new Data(
                new Map(
                    int.Parse(ini.Read("width", "Map")),
                    int.Parse(ini.Read("height", "Map"))
                    ),
                (Direction)r.Next(0, 4), Id);

            try
            {
                // 添加一个显示窗口模块
                ShowWindow s = new ShowWindow(
                    Data,
                    int.Parse(ini.Read("interval", "Show")),
                    int.Parse(ini.Read("length", "Show"))
                );
                s.Text = $"Snake Game System {Id}";
                funcs.Add(new ShowWindowThread(s));

                // 添加游戏主线程
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

        public async Task Start()
        {
            isRunning = true;
            var tasks = new List<Task>();

            foreach (var func in funcs)
            {
                tasks.Add(Task.Run(async () => {
                    try
                    {
                        await func.Start();
                    }
                    finally
                    {
                        isRunning = !tasks.Exists(t => t.IsCompleted);
                    }
                }));
            }

            await Task.WhenAny(tasks);
            Stop();
        }

        public void Stop()
        {
            foreach (FunctionInterface func in funcs)
            {
                if (func.IsRunning())
                    func.Stop();
            }
            isRunning = false;
        }

        public bool IsRunning()
        {
            return isRunning;
        }
    }
}
