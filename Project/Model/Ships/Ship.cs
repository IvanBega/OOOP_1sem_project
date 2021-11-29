namespace Project.Model.Ships
{
    public abstract class Ship
    {
        public int Length { get; set; }
        public Direction Direction { get; set; }
        public Position Position { get; set; }
        public int DamageCount { get; set; } = 0;
        public Ship(Position position, Direction direction)
        {
            Position = position;
            Direction = direction;
        }
        public Ship()
        {

        }
    }
}
