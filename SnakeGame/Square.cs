﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    // prosta strukturka (X,Y) dla 
    internal class Square
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Square()
        {
            X = 0;
            Y = 0;
        }
    }
}
