using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Settings
    {
        static public int Width { get; set; }
        static public int Height { get; set; }
        static public int Speed { get; set; }
        static public int Score { get; set; }
        static public int Points { get; set; }
        static public bool GameOver { get; set; }
        static public Direction Dir { get; set; }
        static public int Interval { get; set; }

        public Settings()
        {
            Width = 16;
            Height = 16;
            Speed = 10;
            Score = 0;
            Points = 100;
            GameOver = false;
            Dir = Direction.Up;
        }
    }
}
