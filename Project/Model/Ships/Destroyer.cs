using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model.Ships
{
    public class Destroyer : Ship
    {
        public Destroyer(Position position, Direction direction) : base(position, direction)
        {
            Length = 2;
        }
    }
}
