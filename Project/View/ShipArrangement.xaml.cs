using Project.Model;
using Project.Model.Ships;
using Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project.View
{
    /// <summary>
    /// Interaction logic for ShipArrangement.xaml
    /// </summary>
    public partial class ShipArrangement : Window
    {
        public int[] shipCount { get; set; }
        private int[] shipCountCopy = new int[5];
        private int index = 0;
        private MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        private CellState[,] playerCells = new CellState[10,10];
        public ShipArrangement()
        {
            InitializeComponent();
            
            //DataContext = this;
        }

        public void SetShipCount()
        {
            wnd.enemyShips = RandomSetup();
            Array.Copy(shipCount, shipCountCopy, 5);
            while (shipCount[index] == 0)
            {
                index++;
            }
            drawMiniShip(index + 1);
        }
        private List<Ship> RandomSetup()
        {
            int size, index_i, index_j;
            Direction direction = Direction.Horizontal;
            CellState[,] cells = new CellState[10, 10];
            Random r = new Random();
            List<Ship> ships = new();
            bool flag = true;
            for (int k = 0; k < 5; k++)
            {
                for (int l = 0; l < shipCount[k]; l++)
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
                                if (cells[index_i, index_j + j] == CellState.Occupied || index_j + j >= 10)
                                {
                                    flag = true;
                                    index_j = r.Next(0, 10 - size + 1);
                                    break;
                                }
                            }
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
                                if (cells[index_i + i, index_j] == CellState.Occupied || index_i + i >= 10)
                                {
                                    flag = true;
                                    index_i = r.Next(0, 10 - size + 1);
                                    break;
                                }
                            }
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
            switch (size)
            {
                case 1:
                    return new PatrolBoat(pos, direction);
                case 2:
                    return new Destroyer(pos, direction);
                case 3:
                    return new Submarine(pos, direction);
                case 4:
                    return new Battleship(pos, direction);
                case 5:
                    return new Carrier(pos, direction);
                default:
                    throw new ArgumentOutOfRangeException(String.Format("Failed to initialize ship with size {0}", size));
            }
        }
        private void drawMiniShip(int length)
        {
            MiniShipGrid.Children.Clear();
            for (int i = 0; i < length; i++)
            {
                Rectangle rect = new Rectangle { Fill = Brushes.Black };
                Grid.SetColumn(rect, i + 1);
                Grid.SetRow(rect, 1);
                MiniShipGrid.Children.Add(rect);
            }
        }
        private void PlaceShipBtn_Click(object sender, RoutedEventArgs e)
        {
            int x = 0, y = 0;
            Ship ship;
            bool flag = int.TryParse(xCoord.Text, out x) && int.TryParse(yCoord.Text, out y);
            if (!flag)
            {
                MessageBox.Show("Could not recognize coordinates! Try again...");
                return;
            }
            Position position = new(x - 1, y - 1);
            Direction direction = Direction.Horizontal;
            if (ComboBox.SelectedIndex == 1)
            {
                direction = Direction.Vertical;
            }
            if (!GameBoard.CanPlaceShip(playerCells, x - 1, y - 1, index + 1, direction))
            {
                SoundEffect.PlayErrorSound();
                MessageBox.Show("Can't place ship here!");
                return;
            }
            if (index < 5 && shipCount[index] > 0)
            {
                ship = InitShipBySize(index + 1, position, direction);
                wnd.playerShips.Add(ship);
                GameBoard.DrawShip(ship, ArrangementGrid, playerCells, Brushes.Black, CellState.Occupied);
                shipCount[index]--;
                while (index < 5 && shipCount[index] == 0)
                {
                    index++;
                }
                if (index < 5)
                {
                    drawMiniShip(index + 1);
                }
            }
            if (index == 5)
            {
                PlaceShipBtn.Visibility = Visibility.Hidden;
                ProceedBtn.Visibility = Visibility.Visible;
                MiniShipGrid.Children.Clear();
            }
        }

        private void ProceedBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            wnd.Show();
            wnd.InitGameBoard();
            wnd.Game();
        }

        private void ResetShipBtn_Click(object sender, RoutedEventArgs e)
        {
            index = 0;
            MiniShipGrid.Children.Clear();
            Array.Copy(shipCountCopy, shipCount, 5);
            Array.Clear(playerCells, 0, playerCells.Length);
            PlaceShipBtn.Visibility = Visibility.Visible;
            ProceedBtn.Visibility = Visibility.Hidden;
            wnd.playerShips.Clear();
            ArrangementGrid.Children.Clear();

            while (shipCount[index] == 0)
            {
                index++;
            }
            drawMiniShip(index + 1);
            
        }

        private void RandomizeShipBtn_Click(object sender, RoutedEventArgs e)
        {
            index = 5;
            ArrangementGrid.Children.Clear();
            Array.Copy(shipCountCopy, shipCount, 5);
            Array.Clear(playerCells, 0, playerCells.Length);
            wnd.playerShips.Clear();
            wnd.playerShips = RandomSetup();
            GameBoard.InitGrid(ArrangementGrid, wnd.playerShips, playerCells);

            PlaceShipBtn.Visibility = Visibility.Hidden;
            ProceedBtn.Visibility = Visibility.Visible;
        }
    }
}
