using Project.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ViewModel
{
    public class AI
    {     
        private enum Compass
        {
            North,
            West,
            South,
            East
        }
        private Queue<Position> posToCheck = new();
        CellState[,] cells;
        private Position lastPos;
        private Compass compass;
        Random random = new();
        public double difficulty;
        public AI(CellState[,] cells, double difficulty)
        {
            this.cells = cells;
            this.difficulty = difficulty;
            lastPos = RandomFreePosition();
            compass = GetRandomCompass();
        }
        private List<Position> GetFreeAdjacentPos(int x, int y)
        {
            List<Position> list = new();
            if (x - 1 > 0 && !ShotCell(x - 1, y))
            {
                list.Add(new Position(x - 1, y));
            }
            if (x + 1 < 10 && !ShotCell(x + 1, y))
            {
                list.Add(new Position(x + 1, y));
            }
            if (y - 1 > 0 && !ShotCell(x, y - 1))
            {
                list.Add(new Position(x, y - 1));
            }
            if (y + 1 < 10 && !ShotCell(x, y + 1))
            {
                list.Add(new Position(x, y + 1));
            }
            list.OrderBy(item => random.Next());
            return list;
        }
        private Position RandomFreePosition()
        {
            int x, y;
            double z = random.NextDouble();
            if (z < difficulty)
            {
                return GetOccupiedPosition();
            }
            while (true)
            {
                x = random.Next(0, 10);
                y = random.Next(0, 10);
                if (!ShotCell(x, y))
                {
                    break;
                }
            }
            return new Position(x, y);
        }
        public void AddAdjacentToQueue(Position position)
        {
            List <Position> list = GetFreeAdjacentPos(position.X, position.Y);
            foreach (var pos in list)
            {
                posToCheck.Enqueue(pos);
            }
        }
        private bool ShotCell(int x, int y)
        {
            return cells[x, y] == CellState.ShotMissed || cells[x, y] == CellState.ShotDestroyed || cells[x,y] == CellState.ShotDestroyedRed;
        }
        private Compass GetRandomCompass()
        {
            int option = random.Next(0, 4);
            switch (option)
            {
                case 0:
                    return Compass.North;
                case 1:
                    return Compass.West;
                case 2:
                    return Compass.South;
                case 3:
                    return Compass.East;
            }
            throw new Exception("Could not generate random Compass");
        }
        public Position PredictMove()
        {
            Position pos = null;
            int t = 0;
            while (posToCheck.Count > 0)
            {
                pos = posToCheck.Dequeue();
                if (!ShotCell(pos.X, pos.Y))
                {
                    lastPos = pos;
                    return pos;
                }
            }
            switch (compass)
            {
                case Compass.North:
                    t = lastPos.Y - 2;
                    while (t >= 0)
                    {
                        if (!ShotCell(lastPos.X, t))
                        {
                            pos = new Position(lastPos.X, t);
                            break;
                        }
                        else
                        {
                            t -= 1;
                        }
                    }
                    break;
                case Compass.South:
                    t = lastPos.Y + 2;
                    while (t < 10)
                    {
                        if (!ShotCell(lastPos.X, t))
                        {
                            pos = new Position(lastPos.X, t);
                            break;
                        }
                        else
                        {
                            t += 1;
                        }
                    }
                    break;
                case Compass.West:
                    t = lastPos.X - 2;
                    while (t >= 0)
                    {
                        if (!ShotCell(t, lastPos.Y))
                        {
                            pos = new Position(t, lastPos.Y);
                            break;
                        }
                        else
                        {
                            t -= 1;
                        }
                    }
                    break;
                case Compass.East:
                    t = lastPos.X + 2;
                    while (t < 10)
                    {
                        if (!ShotCell(t, lastPos.Y))
                        {
                            pos = new Position(t, lastPos.Y);
                            break;
                        }
                        else
                        {
                            t += 1;
                        }
                    }
                    break;
            }
            if (t < 0 || t > 9)
            {
                pos = RandomFreePosition();
                compass = GetRandomCompass();
            }
            //pos = RandomFreePosition();
            lastPos = pos;
            return pos;
        }
        public int GetFreePosCount()
        {
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (cells[i, j] == CellState.ShotDestroyed)
                        count++;
                }
            }
            return count;
        }
        private Position GetOccupiedPosition()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (cells[i,j] == CellState.Occupied)
                    {
                        return new Position(i, j);
                    }
                }
            }
            return null;
        }
    }
}
