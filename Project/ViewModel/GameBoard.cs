using Project.Model;
using Project.Model.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Project.ViewModel
{
    public static class GameBoard
    {
        public static void InitGrid(Grid grid, List<Ship> ships, CellState[,] cells)
        {
            foreach (Ship s in ships)
            {
                DrawShip(s, grid, cells, Brushes.Black, CellState.Occupied);
            }
        }
        public static void DrawShip(Ship ship, Grid grid, CellState[,] cells, SolidColorBrush fill, CellState cellState)
        {
            if (ship.Direction == Direction.Horizontal)
            {
                for (int i = ship.Position.X; i < ship.Length + ship.Position.X; i++)
                {
                    Rectangle rect = new()
                    {
                        Height = 30,
                        Width = 30,
                        Fill = fill
                    };
                    Grid.SetColumn(rect, i);
                    Grid.SetRow(rect, ship.Position.Y);
                    grid.Children.Add(rect);
                    cells[i, ship.Position.Y] = cellState;
                }
            }
            else
            {
                for (int j = ship.Position.Y; j < ship.Length + ship.Position.Y; j++)
                {
                    Rectangle rect = new()
                    {
                        Height = 30,
                        Width = 30,
                        Fill = fill
                    };
                    Grid.SetColumn(rect, ship.Position.X);
                    Grid.SetRow(rect, j);
                    grid.Children.Add(rect);
                    cells[ship.Position.X, j] = CellState.Occupied;
                }
            }
        }
        public static Ship GetShipByPos(List<Ship> ships, int x, int y)
        {
            foreach (Ship ship in ships)
            {
                if (ship.Direction == Direction.Horizontal)
                {
                    if (y == ship.Position.Y && x >= ship.Position.X && x < ship.Position.X + ship.Length)
                    {
                        return ship;
                    }
                }
                else
                {
                    if (x == ship.Position.X && y >= ship.Position.Y && y < ship.Position.Y + ship.Length)
                    {
                        return ship;
                    }
                }
            }
            return null;
        }
        public static bool CanPlaceShip(CellState[,] cells, int x, int y, int shipLength, Direction shipDirection)
        {
            if (shipDirection == Direction.Horizontal)
            {
                if (shipLength >= 10 - x + 1)
                    return false;
                for (int i = x; i < x + shipLength; i++)
                {
                    if (cells[i, y] == CellState.Occupied)
                        return false;
                }
                return true;
            }
            // vertical
            if (shipLength >= 10 - y + 1)
                return false;
            for (int j = y; j < y + shipLength; j++)
            {
                if (cells[x, j] == CellState.Occupied)
                    return false;
            }
            return true;
        }
        public static void GetPosByClick(Point gridPos, Point p, out int row, out int column)
        {
            row = (int)(p.Y - gridPos.Y) / 30;
            column = (int)(p.X - gridPos.X) / 30;
        }
    }
}
