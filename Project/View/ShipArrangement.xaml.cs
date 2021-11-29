using Project.Model;
using Project.Model.Ships;
using Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Project.View
{
    /// <summary>
    /// Interaction logic for ShipArrangement.xaml
    /// </summary>
    public partial class ShipArrangement : Window
    {
        public int[] ShipCount { get; set; }
        private readonly int[] shipCountCopy = new int[5];
        private int index = 0;
        private readonly MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        private readonly CellState[,] playerCells = new CellState[10, 10];
        private Point gridPos = new(80, 60);
        public ShipArrangement()
        {
            InitializeComponent();
            ShipCount = wnd.ShipCount;
        }
        public void SetShipCount()
        {
            index = 0;
            if (ShipCount == null || ShipCount.Length == 0)
            {
                //shipCount = new int[5] { 1, 0, 0, 0, 0 };
            }
            wnd.EnemyShips = RandomSetup();
            Array.Copy(ShipCount, shipCountCopy, 5);
            while (ShipCount[index] == 0)
            {
                index++;
            }
            DrawMiniShip(index + 1);
        }
        private List<Ship> RandomSetup()
        {
            int size, index_i, index_j;
            Direction direction;
            CellState[,] cells = new CellState[10, 10];
            Random r = new();
            List<Ship> ships = new();
            bool flag = true;
            for (int k = 0; k < 5; k++)
            {
                for (int l = 0; l < ShipCount[k]; l++)
                {
                    flag = true;
                    size = k + 1;
                    if (r.Next(0, 2) == 0)
                    {
                        direction = Direction.Vertical;
                    }
                    else
                    {
                        direction = Direction.Horizontal;
                    }

                    if (direction == Direction.Horizontal)
                    {
                        index_i = r.Next(0, 10);
                        index_j = r.Next(0, 10 - size + 1);
                        while (flag)
                        {
                            flag = false;

                            for (int j = 0; j < size; j++)
                            {
                                if (index_j + j >= 10 || cells[index_i, index_j + j] == CellState.Occupied)
                                {
                                    flag = true;
                                    index_j = r.Next(0, 10 - size + 1);
                                    break;
                                }
                            }

                        }
                        for (int j = 0; j < size; j++)
                        {
                            cells[index_i, index_j + j] = CellState.Occupied;
                        }
                    }
                    else
                    {
                        index_i = r.Next(0, 10 - size + 1);
                        index_j = r.Next(0, 10);
                        while (flag)
                        {
                            flag = false;

                            for (int i = 0; i < size; i++)
                            {
                                if (index_i + i >= 10 || cells[index_i + i, index_j] == CellState.Occupied)
                                {
                                    flag = true;
                                    index_i = r.Next(0, 10 - size + 1);
                                    break;
                                }
                            }
                        }
                        for (int i = 0; i < size; i++)
                        {
                            cells[index_i + i, index_j] = CellState.Occupied;
                        }
                    }
                    Position pos = new(index_j, index_i);
                    ships.Add(InitShipBySize(size, pos, direction));
                }
            }
            return ships;
        }
        private Ship InitShipBySize(int size, Position pos, Direction direction)
        {
            return size switch
            {
                1 => new PatrolBoat(pos, direction),
                2 => new Destroyer(pos, direction),
                3 => new Submarine(pos, direction),
                4 => new Battleship(pos, direction),
                5 => new Carrier(pos, direction),
                _ => throw new ArgumentOutOfRangeException(String.Format("Failed to initialize ship with size {0}", size)),
            };
        }
        private void DrawMiniShip(int length)
        {
            MiniShipGrid.Children.Clear();
            for (int i = 0; i < length; i++)
            {
                Rectangle rect = new() { Fill = Brushes.Black };
                Grid.SetColumn(rect, i + 1);
                Grid.SetRow(rect, 1);
                MiniShipGrid.Children.Add(rect);
            }
            for (int i = 1; i < length + 1; i++)
            {
                Border b = new();
                b.BorderThickness = new Thickness { Bottom = 5, Top = 5, Left = 0, Right = 0 };
                b.BorderBrush = Brushes.White;
                Grid.SetColumn(b, i);
                Grid.SetRow(b, 1);
                MiniShipGrid.Children.Add(b);
            }
            Border right = new();
            right.BorderThickness = new Thickness { Right = 5 };
            right.BorderBrush = Brushes.White;
            Grid.SetRow(right, 1);
            Grid.SetColumn(right, length);
            MiniShipGrid.Children.Add(right);

            Border left = new();
            left.BorderThickness = new Thickness { Left = 5 };
            left.BorderBrush = Brushes.White;
            Grid.SetRow(left, 1);
            Grid.SetColumn(left, 1);
            MiniShipGrid.Children.Add(left);
        }
        private void PlaceShip(int x, int y)
        {
            Ship ship;
            Position position = new(x, y);
            Direction direction = Direction.Horizontal;
            if (ComboBox.SelectedIndex == 1)
            {
                direction = Direction.Vertical;
            }
            if (!GameBoard.CanPlaceShip(playerCells, x, y, index + 1, direction))
            {
                SoundEffect.PlayErrorSound();
                MessageBox.Show("Can't place ship here!");
                return;
            }
            if (index < 5 && ShipCount[index] > 0)
            {
                ship = InitShipBySize(index + 1, position, direction);
                wnd.PlayerShips.Add(ship);
                GameBoard.DrawShip(ship, ArrangementGrid, playerCells, Brushes.Black, CellState.Occupied);
                GameBoard.DrawShipBorders(ship, ArrangementGrid, Brushes.White, playerCells);
                ShipCount[index]--;
                while (index < 5 && ShipCount[index] == 0)
                {
                    index++;
                }
                if (index < 5)
                {
                    DrawMiniShip(index + 1);
                }
            }
            if (index == 5)
            {
                ProceedBtn.Visibility = Visibility.Visible;
                MiniShipGrid.Children.Clear();
            }
        }
        private void ProceedBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            wnd.Show();
            wnd.InitGameBoard();
        }
        private void ResetShipBtn_Click(object sender, RoutedEventArgs e)
        {
            ResetArrangement();

        }
        private void RandomizeShipBtn_Click(object sender, RoutedEventArgs e)
        {
            ResetPlayerCells();
            index = 5;
            ArrangementGrid.Children.Clear();
            Array.Copy(shipCountCopy, ShipCount, 5);
            Array.Clear(playerCells, 0, playerCells.Length);
            MiniShipGrid.Children.Clear();
            wnd.PlayerShips.Clear();
            wnd.PlayerShips = RandomSetup();
            GameBoard.InitGrid(ArrangementGrid, wnd.PlayerShips, playerCells, Brushes.Black);

            ProceedBtn.Visibility = Visibility.Visible;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(this);
            int row, column;
            if (pos.X > gridPos.X && pos.X < gridPos.X + 300 && pos.Y > gridPos.Y && pos.Y < gridPos.Y + 300)
            {
                GetPosByClick(pos, out row, out column);
                PlaceShip(column, row);
            }
        }
        private void GetPosByClick(Point p, out int row, out int column)
        {
            row = (int)(p.Y - gridPos.Y) / 30;
            column = (int)(p.X - gridPos.X) / 30;
        }
        private void ResetPlayerCells()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    playerCells[i, j] = CellState.Free;
                }
            }
        }
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            ResetArrangement();
            Hide();
            wnd.menu.Show();
        }
        private void ResetArrangement()
        {
            ResetPlayerCells();
            index = 0;
            MiniShipGrid.Children.Clear();
            Array.Copy(shipCountCopy, ShipCount, 5);
            Array.Clear(playerCells, 0, playerCells.Length);
            ProceedBtn.Visibility = Visibility.Hidden;
            wnd.PlayerShips.Clear();
            ArrangementGrid.Children.Clear();

            while (ShipCount[index] == 0)
            {
                index++;
            }
            DrawMiniShip(index + 1);
        }
    }
}
