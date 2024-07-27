using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame.Interface
{
    public enum Direction
    {
        up,
        down,
        left,
        right
    }

    public class Map
    {
        public int[,] Data;
        public int Width;
        public int Height;

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Data = new int[height, width];
        }
    }
    public interface DataInterface
    {
        int Id();
        int Map(int x, int y);
        int MapWidth();
        int MapHeight();
        float Score();
        void SetMap(int x, int y, int data);
        Direction Direction();
        Direction LastDirection();
        void SetDirection(Direction direction);
        void SetLastDirection(Direction direction);

        void SetScore(float score);
    }
}
