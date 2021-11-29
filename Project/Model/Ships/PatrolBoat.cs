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
