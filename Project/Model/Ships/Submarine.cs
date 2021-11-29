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
