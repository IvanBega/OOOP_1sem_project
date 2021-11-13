using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
        public List<Ship> playerShips { get; set; } = new();
        public List<Ship> enemyShips { get; set; }
        public CellState[,] playerCells { get; set; } = new CellState[10, 10];
        public CellState[,] enemyCells { get; set; } = new CellState[10, 10];
        private Settings s = new();
        private MoveType currentMove = MoveType.PlayerMove;
        public event Action FinishedMove;
        private SoundPlayer sp = new SoundPlayer(@"D:\Программирование\CSharp\WPF\Sounds\shoot1.wav");
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            sp.Load();
            Hide();
            s.Show();
        }
        public void InitGameBoard()
        {
            InitGrid(PlayerGrid, playerShips, playerCells);
            InitGrid(EnemyGrid, enemyShips, enemyCells);
            FinishedMove += Game;
        }

        private void InitGrid(Grid grid, List<Ship> ships, CellState[,] cells)
        {
            foreach (Ship s in ships)
            {
                DrawShip(s, grid, cells, Brushes.Black);
            }
        }

        public static void DrawShip(Ship ship, Grid grid, CellState[,] cells, SolidColorBrush fill)
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
                        Fill = fill
                    };
                    Grid.SetColumn(rect, ship.Position.X);
                    Grid.SetRow(rect, j);
                    grid.Children.Add(rect);
                    cells[ship.Position.X, j] = CellState.Occupied;
                }
            }
        }
        private Ship GetShipByPos(List<Ship> ships, int x, int y)
        {
            foreach(Ship ship in ships)
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
        public bool Shoot(int x, int y, MoveType moveType) 
        {
            sp.Play();
            CellState[,] opponentCellState = playerCells;
            List<Ship> opponentShips = playerShips;
            Grid opponentGrid = PlayerGrid;
            if (moveType == MoveType.PlayerMove)
            {
                opponentCellState = enemyCells;
                opponentShips = enemyShips;
                opponentGrid = EnemyGrid;
            }

            if (opponentCellState[x,y] == CellState.Free)
            {
                Rectangle rect = new()
                {
                    Height = 30,
                    Width = 30,
                    Fill = Brushes.Gray
                };
                Grid.SetColumn(rect, x);
                Grid.SetRow(rect, y);
                opponentGrid.Children.Add(rect);
                opponentCellState[x,y] = CellState.ShotMissed;
            }

            if (opponentCellState[x,y] == CellState.Occupied)
            {
                Ship damagedShip = GetShipByPos(opponentShips, x, y);
                damagedShip.DamageCount += 1;
                if (damagedShip.DamageCount == damagedShip.Length)
                {
                    DrawShip(damagedShip, EnemyGrid, opponentCellState, Brushes.Red); // marking destroyed ship in red
                }
                else
                {
                    Rectangle rect = new()
                    {
                        Height = 30,
                        Width = 30,
                        Fill = Brushes.Yellow
                    };
                    Grid.SetColumn(rect, x);
                    Grid.SetRow(rect, y);
                    opponentGrid.Children.Add(rect);
                }
                opponentCellState[x, y] = CellState.ShotDestroyed;
                return true;
            }
            return false;
        }
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            btn1.Visibility = Visibility.Hidden;
            int x = int.Parse(xPosBox.Text);
            int y = int.Parse(yPosBox.Text);
            bool shootResult;
            if (enemyCells[x-1,y-1] == CellState.ShotMissed || enemyCells[x-1,y-1] == CellState.ShotDestroyed)
            {
                MessageBox.Show("You've already shot this position! Try again...");
                btn1.Visibility = Visibility.Visible;
                return;
            }
            shootResult = Shoot(x - 1, y - 1, MoveType.PlayerMove);
            if (shootResult)
            {
                currentMove = MoveType.PlayerMove;
            }
            else
            {
                currentMove = MoveType.EnemyMove;
            }
            FinishedMove?.Invoke();
        }
        public async void Game()
        {
            if (currentMove == MoveType.PlayerMove)
            {
                btn1.Visibility = Visibility.Visible;
            }
            else
            {
                Random r = new();
                int delay = r.Next(900, 2100);
                await Task.Delay(delay);
                int x, y;
                bool shootResult;
                PredictMove(out x, out y);
                shootResult = Shoot(x, y, MoveType.EnemyMove);
                if (shootResult)
                {
                    currentMove = MoveType.EnemyMove;
                }
                else
                {
                    currentMove = MoveType.PlayerMove;
                }
                FinishedMove?.Invoke();
            }
        }
        private void PredictMove(out int x, out int y)
        {
            Random r = new Random();
            while (true)
            {
                x = r.Next(0, 10);
                y = r.Next(0, 10);
                if (playerCells[x,y] != CellState.ShotMissed && playerCells[x,y] != CellState.ShotDestroyed)
                {
                    break;
                }
            }
        }
        private void PlaySound()
        {
            sp.Play();
        }
    }

}
