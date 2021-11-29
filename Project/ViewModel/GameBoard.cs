using Project.Model;
using Project.Model.Ships;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Project.ViewModel
{
    public static class GameBoard
    {
        public static void InitGrid(Grid grid, List<Ship> ships, CellState[,] cells, SolidColorBrush fill)
        {
            foreach (Ship s in ships)
            {
                DrawShip(s, grid, cells, fill, CellState.Occupied);
                DrawShipBorders(s, grid, Brushes.White, cells);
            }
        }
        public static void DrawShipBorders(Ship ship, Grid grid, SolidColorBrush fill, CellState[,] cells)
        {
            if (ship.Direction == Direction.Horizontal)
            {
                for (int i = ship.Position.X; i < ship.Length + ship.Position.X; i++)
                {
                    if (cells[i, ship.Position.Y] == CellState.ShotDestroyed)
                        continue;
                    Border b = new();
                    b.BorderThickness = new Thickness { Bottom = 5, Top = 5, Left = 0, Right = 0 };
                    b.BorderBrush = fill;
                    Grid.SetColumn(b, i);
                    Grid.SetRow(b, ship.Position.Y);
                    grid.Children.Add(b);
                }
                if (cells[ship.Position.X + ship.Length - 1, ship.Position.Y] != CellState.ShotDestroyed)
                {
                    Border right = new();
                    right.BorderThickness = new Thickness { Right = 5 };
                    right.BorderBrush = fill;
                    Grid.SetRow(right, ship.Position.Y);
                    Grid.SetColumn(right, ship.Position.X + ship.Length - 1);
                    grid.Children.Add(right);
                }

                if (cells[ship.Position.X, ship.Position.Y] != CellState.ShotDestroyed)
                {
                    Border left = new();
                    left.BorderThickness = new Thickness { Left = 5 };
                    left.BorderBrush = fill;
                    Grid.SetRow(left, ship.Position.Y);
                    Grid.SetColumn(left, ship.Position.X);
                    grid.Children.Add(left);
                }
            }
            else
            {
                for (int j = ship.Position.Y; j < ship.Length + ship.Position.Y; j++)
                {
                    if (cells[ship.Position.X, j] == CellState.ShotDestroyed)
                        continue;
                    Border b = new();
                    b.BorderThickness = new Thickness { Left = 5, Right = 5 };
                    b.BorderBrush = fill;
                    Grid.SetColumn(b, ship.Position.X);
                    Grid.SetRow(b, j);
                    grid.Children.Add(b);
                }
                if (cells[ship.Position.X, ship.Position.Y] != CellState.ShotDestroyed)
                {
                    Border top = new();
                    top.BorderThickness = new Thickness { Top = 5 };
                    top.BorderBrush = fill;
                    Grid.SetColumn(top, ship.Position.X);
                    Grid.SetRow(top, ship.Position.Y);
                    grid.Children.Add(top);
                }

                if (cells[ship.Position.X, ship.Position.Y + ship.Length - 1] != CellState.ShotDestroyed)
                {
                    Border bottom = new();
                    bottom.BorderThickness = new Thickness { Bottom = 5 };
                    bottom.BorderBrush = fill;
                    Grid.SetColumn(bottom, ship.Position.X);
                    Grid.SetRow(bottom, ship.Position.Y + ship.Length - 1);
                    grid.Children.Add(bottom);
                }
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
                    cells[ship.Position.X, j] = cellState;
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
        public static void DrawCellsOnGrid(Grid grid, CellState[,] cells, bool drawBlack)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    switch (cells[i, j])
                    {
                        case CellState.ShotMissed:
                            Rectangle rect1 = new()
                            {
                                Height = 30,
                                Width = 30,
                                Fill = Brushes.Gray
                            };
                            Grid.SetColumn(rect1, i);
                            Grid.SetRow(rect1, j);
                            grid.Children.Add(rect1);
                            break;
                        case CellState.ShotDestroyed:
                            Rectangle rect = new()
                            {
                                Height = 30,
                                Width = 30,
                                Fill = Brushes.Yellow
                            };
                            Grid.SetColumn(rect, i);
                            Grid.SetRow(rect, j);
                            grid.Children.Add(rect);
                            break;
                        case CellState.ShotDestroyedRed:
                            Rectangle rect2 = new()
                            {
                                Height = 30,
                                Width = 30,
                                Fill = Brushes.Red
                            };
                            Grid.SetColumn(rect2, i);
                            Grid.SetRow(rect2, j);
                            grid.Children.Add(rect2);
                            break;
                        case CellState.Occupied:
                            if (!drawBlack)
                                continue;
                            Rectangle rect3 = new()
                            {
                                Height = 30,
                                Width = 30,
                                Fill = Brushes.Black
                            };
                            Grid.SetColumn(rect3, i);
                            Grid.SetRow(rect3, j);
                            grid.Children.Add(rect3);
                            break;
                    }
                }
            }
        }
        public static int GetOccupiedCells(CellState[,] cells)
        {
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (cells[i, j] == CellState.Occupied)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
