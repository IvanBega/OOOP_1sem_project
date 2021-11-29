namespace Project.Model.Ships
{
    public class Battleship : Ship
    {
        public Battleship(Position position, Direction direction) : base(position, direction)
        {
            Length = 4;
        }
    }
}
