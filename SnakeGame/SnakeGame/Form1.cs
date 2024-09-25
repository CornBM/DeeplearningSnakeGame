using SnakeGame.Class;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        public GameSystemManager M;
        public Form1()
        {
            InitializeComponent();
            M = new GameSystemManager(4, 5555, "./data/config.ini");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            M.StartNewGame();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("E:\\收藏\\小项目\\snake\\DeeplearningSnakeGame\\SnakeGame\\SnakeGame\\bin\\Debug\\data\\config.ini");
        }

        
    }
}
