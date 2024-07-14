using SnakeGame.Class;
using SnakeGame.Interface;
using SnakeGame.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Xml.Linq;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        public GameSystemManager M;
        public Form1()
        {
            InitializeComponent();
            M = new GameSystemManager(2, 1145);
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
