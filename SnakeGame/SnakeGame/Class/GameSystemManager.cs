using SnakeGame.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame.Class
{
    public class GameSystemManager
    {
        public Hashtable games;
        public bool[] idAvailability;

        public FunctionInterface network;

        public GameSystemManager(int maxNum, int port)
        {
            games = new Hashtable();
            idAvailability = Enumerable.Repeat(true, maxNum).ToArray();
            network = new NetworkInterface(ProcessMessage, port);
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

        public void StartNewGame()
        {
            int id = GetId();
            if (id == -1)
            {
                MessageBox.Show("Not enough numbers of id!", "Warning!", MessageBoxButtons.OK);
                return;
            }
            FunctionInterface game = new GameSystem("./data/config.ini", id);
            games.Add(id, game);
            idAvailability[id] = false;
            game.Start();
        }

        public void StopGame(int id)
        {
            games.Remove(id);
            idAvailability[id] = true;
        }

        public void ProcessMessage(string message)
        {
            Console.WriteLine(message);
        }
    }


}
