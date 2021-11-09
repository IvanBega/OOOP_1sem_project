using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model.Ships
{
    public class Submarine : Ship
    {
        public Submarine(Position position, Direction direction) : base(position, direction)
        {
            Length = 3;
        }
    }
}
