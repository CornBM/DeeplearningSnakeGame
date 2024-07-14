using SnakeGame.Class;
using SnakeGame.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame.Gui
{
    public partial class ShowWindow : Form
    {
        private DataInterface Data;
        private Graphics GI;
        private Graphics G;
        private int length;

        private Bitmap i;

        private Thread showThread;
        private bool isRunning = false;
        private int interval;
        public ShowWindow(DataInterface data, int interval, int length)
        {
            InitializeComponent();
            Data = data;
            this.interval = interval;
            this.length = length;
        }

        public void DrawBlock(int x, int y, Color c)
        {
            GI.FillRectangle(new SolidBrush(c), x * length, y * length, length, length);
            GI.DrawRectangle(new Pen(Color.Black), x * length, y * length, length-1, length-1);
        }

        public void UpdateImage()
        {
            int size = Data.MapWidth() * Data.MapHeight();
            for(int j = 0; j < Data.MapHeight(); j++)
            {
                for(int i = 0; i < Data.MapWidth(); i++)
                {
                    Color c = Color.White;
                    if (Data.Map(i, j) >= 1)
                    {
                        int temp = (int)(128 - 128 * ((float)Data.Map(i, j) / size));
                        c = Color.FromArgb(temp, temp,temp);
                        
                    }
                    else if (Data.Map(i, j) == -1)
                    {
                        c = Color.Pink;
                    }
                    DrawBlock(i, j, c);
                    //Console.Write($"{Data.Map(i, j)}  ");
                }
                //Console.WriteLine();
            }
        }

        public void Showwindow()
        {
            UpdateImage();
            G.DrawImage(i, 20, 20);
        }

        private void ShowWindow_Load(object sender, EventArgs e)
        {
            i = new Bitmap(Data.MapWidth() * length, Data.MapHeight() * length + 30);
            this.Size = new Size(Data.MapWidth() * length + 60, Data.MapHeight() * length + 80);
            this.Refresh();
            GI = Graphics.FromImage(i);
            G = this.CreateGraphics();
            Start();
        }

        private void ShowWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            isRunning = false;
            if (showThread != null && showThread.IsAlive)
            {
                showThread.Join();
            }
        }

        private void ShowWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key = Char.ToLower(e.KeyChar);
            Direction d = Data.LastDirection();

            switch (key)
            {
                case 'w':
                    if (d != Direction.down)
                    {
                        Data.SetDirection(Direction.up);
                    }
                    break;
                case 's':
                    if (d != Direction.up)
                    {
                        Data.SetDirection(Direction.down);
                    }
                    break;
                case 'a':
                    if (d != Direction.right)
                    {
                        Data.SetDirection(Direction.left);
                    }
                    break;
                case 'd':
                    if (d != Direction.left)
                    {
                        Data.SetDirection(Direction.right);
                    }
                    break;
                default:
                    // 如果按下的键不是w, s, a, d，则不执行任何操作
                    break;
            }
        }

        public void Start()
        {
            isRunning = true;
            showThread = new Thread(() => {
                while (isRunning)
                {
                    Showwindow();
                    Thread.Sleep(interval);
                }
            });
            showThread.Start();
        }
    }
}
