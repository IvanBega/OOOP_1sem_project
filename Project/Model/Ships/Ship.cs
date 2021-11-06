using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Model;
namespace Project.Model.Ships
{
    public abstract class Ship
    {
        public int Length { get; set; }
        public Orientation Orientation { get; set; }
        public Position Position { get; set; }
        public int DamageCount { get; set; } = 0;
        public Ship(Position position)
        {
            Position = position;
        }
        public Ship()
        {

        }
    }
}
