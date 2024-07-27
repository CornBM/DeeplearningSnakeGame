using SnakeGame.Class;
using System;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        public GameSystemManager M;
        public Form1()
        {
            InitializeComponent();
            M = new GameSystemManager(4, 5555);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            M.StartNewGame();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        
    }
}
