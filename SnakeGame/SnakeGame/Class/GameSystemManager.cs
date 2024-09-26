using SnakeGame.Interface;
using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame.Class
{
    public class GameSystemManager
    {
        public Hashtable games; // id -> game  <int, GameSystem>
        public bool[] idAvailability; // 限制同时进行游戏的数量

        public NetworkInterface network; // 提供网络接口让ai玩游戏
        private int port;
        private string _configPath;

        public GameSystemManager(int maxNum, int port, string configPath)
        {
            _configPath = configPath;
            games = new Hashtable();
            idAvailability = Enumerable.Repeat(true, maxNum).ToArray();
            network = new NetworkInterface(ProcessMessage, port);
            _ = network.Start();
        }

        // 获取可用id
        private int GetId()
        {
            int l = idAvailability.Length;
            for (int i = 0; i < l; i++)
            {
                if (idAvailability[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public int StartNewGame()
        {
            int id = GetId();
            if (id == -1)
            {
                MessageBox.Show("Not enough numbers of id!", "Warning!", MessageBoxButtons.OK);
                return -1;
            }
            // 如果有可用id，则创建一个游戏
            GameSystem game = new GameSystem(_configPath, id);
            games.Add(id, game); // 添加到games中，以便管理
            Task.Run(async () => {
                // 启动游戏 
                idAvailability[id] = false;
                await game.Start();
                // 等待游戏结束后，移除游戏并释放id
                games.Remove(id);
                idAvailability[id] = true;
            });

            return id;
        }

        // 处理从网络接口收到的指令
        public void ProcessMessage(string message)
        {
            string[] data = message.Split(' ');
            int id = -1;
            GameSystem game = null;
            try
            {
                id = int.Parse(data[0]);
                if (!idAvailability[id])
                {
                    Direction d = (Direction)int.Parse(data[1]);
                    game = (GameSystem)games[id];
                    game.Data.SetDirection(d);
                    network.Send($"{id} {game.Data.Score()} {(int)game.Data.Direction()} {game.Data.SerializeMap()}");
                }
                else
                {
                    Console.WriteLine($"id:{id}");
                    network.Send($"{id} 0 0 over");
                }
            }
            catch
            {
                switch (data[0][0])
                {
                    case '+':
                        id = StartNewGame();
                        game = (GameSystem)games[id];
     
                        network.Send($"{id} {game.Data.Score()} {(int)game.Data.Direction()} {game.Data.SerializeMap()}");
                        break;
                    case '-':
                        id = int.Parse(data[1]);
                        if (games.ContainsKey(id))
                        {
                            game = (GameSystem)games[id];
                            game.Stop();
                            network.Send(id + " over");
                        }
                        break;
                }
            }
        }
    }
}
