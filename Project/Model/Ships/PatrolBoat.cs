using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model.Ships
{
    public class PatrolBoat : Ship
    {
        public PatrolBoat(Position position, Direction direction) : base(position, direction)
        {
            Length = 1;
        }
    }
}
