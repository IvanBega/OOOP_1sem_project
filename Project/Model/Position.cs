namespace Project.Model
{
    public class Position
    {
        public int X { get; init; }
        public int Y { get; init; }
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Position()
        {
        }
        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Position pos = (Position)obj;
                return (pos.X == X) && (pos.Y == Y);
            }
        }
    }
}
