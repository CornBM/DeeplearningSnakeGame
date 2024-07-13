using SnakeGame.Class;
using SnakeGame.Interface;
using SnakeGame.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        public FunctionInterface gs;
        public Form1()
        {
            InitializeComponent();     
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gs = new GameSystem("./data/config.ini");
            gs.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        
    }
}
