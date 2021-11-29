namespace Project.Model.Ships
{
    public class Carrier : Ship
    {
        public Carrier(Position position, Direction direction) : base(position, direction)
        {
            Length = 5;
        }
    }
}
