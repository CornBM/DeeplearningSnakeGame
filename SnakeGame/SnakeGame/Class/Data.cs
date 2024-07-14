using SnakeGame.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeGame.Class
{
    internal class Data : DataInterface
    {
        private Map map;
        private Direction direction;
        private Direction lastDirection;
        private Semaphore mapLock;
        private Semaphore directionLock;

        public Data(Map map, Direction direction)
        {
            this.map = map;
            this.direction = direction;
            mapLock = new Semaphore(1, 1);
            directionLock = new Semaphore(1, 1);
        }

        public Direction Direction()
        {
            directionLock.WaitOne();
            Direction result = direction;
            directionLock.Release();
            return result;
        }

        public Direction LastDirection()
        {
            directionLock.WaitOne();
            Direction result = lastDirection;
            directionLock.Release();
            return result;
        }

        public int Map(int x, int y)
        {
            int result = map.Data[y, x];
            return result;
        }

        public int MapWidth()
        {
            return map.Width;
        }

        public int MapHeight()
        {
            return map.Height;
        }

        public void SetDirection(Direction direction)
        {
            directionLock.WaitOne();
            this.direction = direction;
            directionLock.Release();
        }

        public string SerializeMap()
        {
            if (map.Data == null) return "";

            // 初始化StringBuilder用于构建最终的字符串
            StringBuilder stringBuilder = new StringBuilder();

            // 遍历数组的每一行
            for (int i = 0; i < map.Data.GetLength(0); i++)
            {
                // 遍历行中的每个元素
                for (int j = 0; j < map.Data.GetLength(1); j++)
                {
                    // 将元素添加到StringBuilder中，元素之间用逗号分隔
                    stringBuilder.Append(map.Data[i, j]);
                    if (j < map.Data.GetLength(1) - 1) // 如果不是行的最后一个元素，添加逗号
                    {
                        stringBuilder.Append(",");
                    }
                }
                // 如果不是最后一行，添加换行符
                if (i < map.Data.GetLength(0) - 1)
                {
                    stringBuilder.Append("\n");
                }
            }

            // 返回构建好的字符串
            return stringBuilder.ToString();
        }

        public void SetLastDirection(Direction direction)
        {
            directionLock.WaitOne();
            this.lastDirection = direction;
            directionLock.Release();
        }

        public void SetMap(int x, int y, int data)
        {
            mapLock.WaitOne();
            map.Data[y, x] = data;
            mapLock.Release();
        }
    }
}
