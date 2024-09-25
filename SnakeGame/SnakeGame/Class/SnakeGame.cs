using SnakeGame.Interface;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

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
        private Task moveThread;
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

        public async Task Start()
        {
            isRunning = true;
            moveThread = Task.Run(()=>{
                while (isRunning)
                {
                    Move();
                    Task.Delay(interval).Wait();
                }
            });
            await moveThread;
        }

        public void Move()
        {
            int tempX = Snake.Head.X;
            int tempY = Snake.Head.Y;
            // 根据当前方向前进一步
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
            // 允许穿墙
            tempX = (tempX + Data.MapWidth()) % Data.MapWidth();
            tempY = (tempY + Data.MapHeight()) % Data.MapHeight();

            // 更新头部坐标
            Snake.Head = new Point(tempX, tempY);
            // 更新记录的“上一次方向”
            Data.SetLastDirection(Data.Direction());
            bool isAte = false;
            // 大于1视为蛇身
            // 如果撞到了自己，则停止
            if (Data.Map(tempX, tempY) > 1)
            {
                Stop();
                return;
            }
            // 如果撞到了食物，则加长，并生成新的食物
            else if (Data.Map(tempX, tempY) == -1)
            {
                Snake.Length += 1;
                // 更新地图数据，把蛇头的位置的值设置为蛇的长度
                Data.SetMap(tempX, tempY, Snake.Length);
                // 标记这次移动吃到了食物
                isAte = true;
            }
            else
            {
                // 更新地图数据，把蛇头的位置的值设置为蛇的长度+1，因为后面要把蛇身整体减一实现移动
                Data.SetMap(tempX, tempY, Snake.Length + 1);
/*                Console.WriteLine($"tempx:{tempX}, tempy:{tempY}");
                Console.WriteLine($"{Data.Map(tempX, tempY)}");*/
            }

            // 如果吃到了食物
            if (isAte)
            {
                // 加分
                Score += 2*Math.Max(Data.MapWidth(),Data.MapHeight());
                // 生成新的食物
                Makefood();
            }
            // 如果没有吃到食物
            else
            {
                int foodCount = 0;
                // 则遍历地图二维数组
                for (int j = 0; j < Data.MapHeight(); j++)
                {
                    for (int i = 0; i < Data.MapWidth(); i++)
                    {
                        // 如果该位置有蛇身，则减一
                        if (Data.Map(i, j) >= 1)
                        {
                            Data.SetMap(i, j, Data.Map(i,j) - 1);
                            if (Data.Map(i, j) == 1)
                            {
                                Snake.Tail.X = i;
                                Snake.Tail.Y = j;
                            }
                        }
                        // 如果该位置有食物，则计数
                        else if (Data.Map(i, j) == -1)
                        {
                            foodCount += 1;
                        }

                    }
                }
                // 如果发现没有食物了，则生成新的食物
                if (foodCount <= 0)
                    Makefood();
            }
            // 每次移动后，减少一分
            Score -= 1;
            // 如果分数小于阈值，则停止游戏
            if (Score < -2 * Math.Max(Data.MapWidth(), Data.MapHeight()))
            {
                Stop();
                return;
            }
            // 更新分数数据
            Data.SetScore(Score - GetDistance(Snake.Head, Snake.Tail));
        }

        // 计算两点之间的距离
        private float GetDistance(Point a, Point b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        // 生成新的食物
        public void Makefood()
        {
            int count = 0;
            // 统计地图中空白位置的个数
            int blankNum = Data.MapWidth() * Data.MapHeight() - Snake.Length;
            // 如果地图中非蛇身的位置个数为0，意味着游戏胜利，则停止游戏
            if (blankNum == 0)
            {
                Stop();
                return;
            }
            // 否则，随机生成一个空白位置作为食物
            Random r = new Random();
            int location = r.Next(0, blankNum);
            // 遍历地图数据
            for (int j = 0; j < Data.MapHeight(); j++)
            {
                for (int i = 0; i < Data.MapWidth(); i++)
                {
                    // 如果该位置是空白的
                    if (Data.Map(i, j) == 0)
                    {
                        // 并且计数器等于随机数，则将该位置设置为食物
                        if (count == location)
                        {
                            Data.SetMap(i, j, -1);
                            break;
                        }
                        // 计数器加一
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
        }

        public bool IsRunning()
        {
            return isRunning;
        }
    }
}
