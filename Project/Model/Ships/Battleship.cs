using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model.Ships
{
    public class Battleship : Ship
    {
        public Battleship(Position position) : base(position)
        {
            Length = 4;
        }
    }
}
