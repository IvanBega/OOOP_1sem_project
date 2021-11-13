using Project.Model;
using Project.Model.Ships;
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
        private int BattleshipLeft;
        private int CarrierLeft;
        private int DestroyerLeft;
        private int PatrolBoatLeft;
        private int SubmarineLeft;
        private int[] shipCount = new int[5];
        public CellState[,] Cells = new CellState[10,10];
        public ShipArrangement()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void PassShipCount(Settings settings)
        {
            BattleshipLeft = settings.BattleshipCount;
            CarrierLeft = settings.CarrierCount;
            DestroyerLeft = settings.DestroyerCount;
            PatrolBoatLeft = settings.PatrolBoatCount;
            SubmarineLeft = settings.SubmarineCount;

            shipCount[0] = settings.BattleshipCount;
            shipCount[1] = settings.CarrierCount;
            shipCount[2] = settings.DestroyerCount;
            shipCount[3] = settings.PatrolBoatCount;
            shipCount[4] = settings.SubmarineCount;
        }
        private List<Ship> RandomSetup(int[] sizes)
        {
            int size, index_i, index_j;
            Direction direction = Direction.Horizontal;
            CellState[,] cells = new CellState[10, 10];
            Random r = new Random();
            List<Ship> ships = new();
            bool flag = true;
            for (int k = 0; k < sizes.Length; k++)
            {
                flag = true;
                size = sizes[k];
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
                switch (size)
                {
                    case 1:
                        ships.Add(new PatrolBoat(pos, direction));
                        break;
                    case 2:
                        ships.Add(new Destroyer(pos, direction));
                        break;
                    case 3:
                        ships.Add(new Submarine(pos, direction));
                        break;
                    case 4:
                        ships.Add(new Battleship(pos, direction));
                        break;
                    case 5:
                        ships.Add(new Carrier(pos, direction));
                        break;
                }
            }
            return ships;
        }
        
        private void PlaceShipBtn_Click(object sender, RoutedEventArgs e)
       {
            
        }

        private void ProceedBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.Show();
            wnd.SetPlayerShips(RandomSetup(new int[5] { 1, 2, 3, 4, 5 }));
            wnd.SetEnemyShips(RandomSetup(new int[5] { 1, 2, 3, 4, 5 }));
            wnd.InitGameBoard();
            wnd.Game();
        }
    }
}
