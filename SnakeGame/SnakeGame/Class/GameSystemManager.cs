using SnakeGame.Interface;
using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SnakeGame.Class
{
    public class GameSystemManager
    {
        public Hashtable games;
        public bool[] idAvailability;

        public NetworkInterface network;
        private int port;

        public GameSystemManager(int maxNum, int port)
        {
            games = new Hashtable();
            idAvailability = Enumerable.Repeat(true, maxNum).ToArray();
            network = new NetworkInterface(ProcessMessage, port);
            network.Start();
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
            FunctionInterface game = new GameSystem("./data/config.ini", id);
            games.Add(id, game);
            idAvailability[id] = false;
            game.Start();
            return id;
        }

        public void StopGame(int id)
        {
            if (!games.ContainsKey(id))
                return;
            games.Remove(id);
            idAvailability[id] = true;
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
                    network.Send($"{id} {game.Data.Score()} {game.Data.SerializeMap()}");
                }
                else
                {
                    Console.WriteLine($"id:{id}");
                    network.Send($"{id} 0 over");
                }
            }
            catch
            {
                switch (data[0][0])
                {
                    case '+':
                        id = StartNewGame();
                        game = (GameSystem)games[id];
     
                        network.Send($"{id} {game.Data.Score()} {game.Data.SerializeMap()}");
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
