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
