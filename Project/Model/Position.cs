using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model
{
    public class Position
    {
        public int X { get; init; }
        public int Y { get; init; }
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Position()
        {
        }
    }
}
