using SnakeGame.Interface;
using System;
using System.Drawing;
using System.Threading;

namespace SnakeGame.Class
{
    public class Snake
    {
        public Point Head;
        public Point Tail;
        public int Length;
        public Snake(Point head, Point tail, int length)
        {
            Head = head;
            Tail = tail;
            Length = length;
        }

        public Snake(DataInterface Data, int length)
        {
            Head = new Point(Data.MapWidth() / 2, Data.MapHeight() / 2);
            Length = length;
            Random r = new Random();
            Point temp;
            switch ((Direction)r.Next(0, 4))
            {
                case Direction.up:
                    temp = new Point(0, 1);
                    break;
                case Direction.down:
                    temp = new Point(0, -1);
                    break;
                case Direction.left:
                    temp = new Point(1, 0);
                    break;
                default:
                    temp = new Point(-1, 0);
                    break;
            }
            Point tail = Head;
            Data.SetMap(Head.X, Head.Y, length);
            for (int i = length - 1; i > 0; i--)
            {
                tail.X += temp.X;
                tail.Y += temp.Y;
                Data.SetMap(tail.X, tail.Y, i);
            }
        }
    }
    public class SnakeGameI : FunctionInterface
    {
        private Snake Snake;
        private Thread moveThread;
        private bool isRunning = false;
        private int interval;

        private DataInterface Data;
        private float Score;

        public SnakeGameI(DataInterface data, Snake snake, int interval)
        {
            Snake = snake;
            Data = data;
            this.interval = interval;
            Score = 0;
            Makefood();
        }

        public void Start()
        {
            isRunning = true;
            moveThread = new Thread(()=>{
                while (isRunning)
                {
                    Move();
                    Thread.Sleep(interval);
                }
            });
            moveThread.Start();
        }

        public void Move()
        {
            int tempX = Snake.Head.X;
            int tempY = Snake.Head.Y;
            switch (Data.Direction())
            {
                case Direction.up:
                    tempY -= 1;
                    break;
                case Direction.down:
                    tempY += 1;
                    break;
                case Direction.left:
                    tempX -= 1;
                    break;
                case Direction.right:
                    tempX += 1;
                    break;
            }
            tempX = (tempX + Data.MapWidth()) % Data.MapWidth();
            tempY = (tempY + Data.MapHeight()) % Data.MapHeight();

            Snake.Head = new Point(tempX, tempY);
            Data.SetLastDirection(Data.Direction());
            bool isAte = false;
            if (Data.Map(tempX, tempY) > 1)
            {
                Stop();
                return;
            }
            else if (Data.Map(tempX, tempY) == -1)
            {
                Snake.Length += 1;

                Data.SetMap(tempX, tempY, Snake.Length);
                isAte = true;
            }
            else
            {
                Data.SetMap(tempX, tempY, Snake.Length + 1);
/*                Console.WriteLine($"tempx:{tempX}, tempy:{tempY}");
                Console.WriteLine($"{Data.Map(tempX, tempY)}");*/
            }

            //scan
            if (isAte)
            {
                //makefood
                Score += 2*Math.Max(Data.MapWidth(),Data.MapHeight());
                Makefood();
            }
            else
            {
                int foodCount = 0;
                for (int j = 0; j < Data.MapHeight(); j++)
                {
                    for (int i = 0; i < Data.MapWidth(); i++)
                    {
                        if (Data.Map(i, j) >= 1)
                        {
                            Data.SetMap(i, j, Data.Map(i,j) - 1);
                            if (Data.Map(i, j) == 1)
                            {
                                Snake.Tail.X = i;
                                Snake.Tail.Y = j;
                            }
                        }else if (Data.Map(i, j) == -1)
                        {
                            foodCount += 1;
                        }

                    }
                }
                if (foodCount == 0)
                    Makefood();
            }
            Score -= 1;
            if (Score < -2 * Math.Max(Data.MapWidth(), Data.MapHeight()))
            {
                Stop();
                return;
            }

            Data.SetScore(Score - GetDistance(Snake.Head, Snake.Tail));
        }

        private float GetDistance(Point a, Point b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public void Makefood()
        {
            int count = 0;
            int blankNum = Data.MapWidth() * Data.MapHeight() - Snake.Length;
            if (blankNum == 0)
            {
                Stop();
                return;
            }
            Random r = new Random();
            int location = r.Next(0, blankNum);
            for (int j = 0; j < Data.MapHeight(); j++)
            {
                for (int i = 0; i < Data.MapWidth(); i++)
                {
                    if (Data.Map(i, j) == 0)
                    {
                        if (count == location)
                        {
                            Data.SetMap(i, j, -1);
                            break;
                        }
                        count += 1;
                    }
                }
                if (count == location)
                    break;
            }
        }

        public void Stop()
        {
            isRunning = false;
            if (moveThread != null && moveThread.IsAlive)
            {
                moveThread.Abort();
            }
        }

        public bool IsRunning()
        {
            return isRunning;
        }
    }
}
