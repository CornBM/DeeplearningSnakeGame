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
            mapLock.WaitOne();
            int result = map.Data[y, x];
            mapLock.Release();
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
