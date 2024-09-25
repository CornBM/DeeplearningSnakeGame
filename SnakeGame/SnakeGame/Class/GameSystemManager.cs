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
        public Hashtable games;
        public bool[] idAvailability;

        public NetworkInterface network;
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
            FunctionInterface game = new GameSystem(_configPath, id);
            games.Add(id, game);
            Task.Run(async () => {
                idAvailability[id] = false;
                await game.Start();
                games.Remove(id);
                idAvailability[id] = true;
            });

            return id;
        }

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
