using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Project.Model;
using Project.Model.Ships;
using Project.View;
namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Ship> playerShips;
        private List<Ship> enemyShips;
        public CellState[,] playerCells = new CellState[10, 10];
        public CellState[,] enemyCells = new CellState[10, 10];
        private Settings s = new();
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            Hide();
            s.Show();
        }
        public void InitGameBoard()
        {
            InitGrid(PlayerGrid, playerShips, playerCells);
            InitGrid(EnemyGrid, enemyShips, enemyCells);
        }

        private void InitGrid(Grid grid, List<Ship> ships, CellState[,] cells)
        {
            foreach (Ship s in ships)
            {
                DrawShip(s, grid, cells);
            }
        }

        private void DrawShip(Ship ship, Grid grid, CellState[,] cells)
        {
            if (ship.Direction == Direction.Horizontal)
            {
                for (int i = ship.Position.X; i < ship.Length + ship.Position.X; i++)
                {
                    Rectangle rect = new()
                    {
                        Height = 30,
                        Width = 30,
                        Fill = Brushes.Black
                    };
                    Grid.SetColumn(rect, i);
                    Grid.SetRow(rect, ship.Position.Y);
                    grid.Children.Add(rect);
                    cells[i, ship.Position.Y] = CellState.Occupied;
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
                        Fill = Brushes.Black
                    };
                    Grid.SetColumn(rect, ship.Position.X);
                    Grid.SetRow(rect, j);
                    grid.Children.Add(rect);
                    cells[ship.Position.X, j] = CellState.Occupied;
                }
            }
        }
        public void SetPlayerShips(List<Ship> ships)
        {
            playerShips = ships;
        }
        public void SetEnemyShips(List<Ship> ships)
        {
            enemyShips = ships;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //
        }
    }

}
