using SnakeGame.Gui;
using SnakeGame.Interface;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SnakeGame.Class
{
    public class GameSystemManager
    {
        public Hashtable games;
        public bool[] idAvailability;

        public NetworkInterface network;

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
            GameSystem game = (GameSystem)games[id];
            game.Stop();
            games.Remove(id);
            idAvailability[id] = true;
        }

        public void ProcessMessage(string message)
        {
            Console.WriteLine(message);
            string[] data = message.Split(' ');
            int id = -1;
            try
            {
                id = int.Parse(data[0]);
                if(games.ContainsKey(id))
                {
                    Direction d = (Direction)int.Parse(data[1]);
                    GameSystem game = (GameSystem)games[id];
                    game.Data.SetDirection(d);
                    network.Send(id + " " + game.Data.SerializeMap());
                }
                else
                {
                    network.Send(id + " over");
                }
            }
            catch
            {
                switch (data[0][0])
                {
                    case '+':
                        id = StartNewGame();
                        GameSystem game = (GameSystem)games[id];
     
                        network.Send(id + " " + game.Data.SerializeMap());
                        break;
                    case '-':
                        id = int.Parse(data[1]);
                        StopGame(id);
                        network.Send(id + " over");
                        break;

                }
            }
        }
    }


}
