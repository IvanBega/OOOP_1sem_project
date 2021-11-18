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
using Project.ViewModel;

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
        private AI EnemyAI;
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
            GameBoard.InitGrid(PlayerGrid, playerShips, playerCells);
            GameBoard.InitGrid(EnemyGrid, enemyShips, enemyCells);
            FinishedMove += Game;
            EnemyAI = new(playerCells);
        }
        public bool Shoot(Position pos, MoveType moveType) 
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

            if (opponentCellState[pos.X,pos.Y] == CellState.Free)
            {
                Rectangle rect = new()
                {
                    Height = 30,
                    Width = 30,
                    Fill = Brushes.Gray
                };
                Grid.SetColumn(rect, pos.X);
                Grid.SetRow(rect, pos.Y);
                opponentGrid.Children.Add(rect);
                opponentCellState[pos.X,pos.Y] = CellState.ShotMissed;
            }

            if (opponentCellState[pos.X,pos.Y] == CellState.Occupied)
            {
                Ship damagedShip = GameBoard.GetShipByPos(opponentShips, pos.X, pos.Y);
                damagedShip.DamageCount += 1;
                if (damagedShip.DamageCount == damagedShip.Length)
                {
                    // GameBoard.DrawShip(damagedShip, EnemyGrid, opponentCellState, Brushes.Red); // marking destroyed ship in red
                    GameBoard.DrawShip(damagedShip, opponentGrid, opponentCellState, Brushes.Red, CellState.ShotDestroyed); // marking destroyed ship in red
                }
                else
                {
                    Rectangle rect = new()
                    {
                        Height = 30,
                        Width = 30,
                        Fill = Brushes.Yellow
                    };
                    Grid.SetColumn(rect, pos.X);
                    Grid.SetRow(rect, pos.Y);
                    opponentGrid.Children.Add(rect);
                }
                opponentCellState[pos.X, pos.Y] = CellState.ShotDestroyed;
                if (moveType == MoveType.EnemyMove)
                {
                    EnemyAI.AddAdjacentToQueue(pos);
                }
                return true;
            }
            return false;
        }
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            btn1.Visibility = Visibility.Hidden;
            int x = 0, y = 0;
            bool result = true;
            result = int.TryParse(xPosBox.Text, out x) && int.TryParse(yPosBox.Text, out y);
            if (!result || x < 0 || y < 0 || x > 10 || y > 10)
            {
                MessageBox.Show("Incorrect input coordinates!");
                btn1.Visibility = Visibility.Visible;
                return;
            }
            bool shootResult;
            if (enemyCells[x-1,y-1] == CellState.ShotMissed || enemyCells[x-1,y-1] == CellState.ShotDestroyed)
            {
                MessageBox.Show("You've already shot this position! Try again...");
                btn1.Visibility = Visibility.Visible;
                return;
            }
            shootResult = Shoot(new Position(x - 1, y - 1), MoveType.PlayerMove);
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
                bool shootResult;
                Position pos;
                //pos = EnemyAI.RandomFreePosition();
                pos = EnemyAI.PredictMove();
                shootResult = Shoot(pos, MoveType.EnemyMove);
                //botX.Content = pos.X;
                botX.Content = EnemyAI.GetFreePosCount();
                botY.Content = pos.Y;
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
        //private void PredictMove(out int x, out int y)
        //{
        //    Random r = new Random();
        //    while (true)
        //    {
        //        x = r.Next(0, 10);
        //        y = r.Next(0, 10);
        //        if (playerCells[x, y] != CellState.ShotMissed && playerCells[x, y] != CellState.ShotDestroyed)
        //        {
        //            break;
        //        }
        //    }
        //}
        private void PlaySound()
        {
            sp.Play();
        }
        
        private bool ShotCell(CellState[,] cells, int x, int y)
        {
            return cells[x, y] == CellState.ShotMissed || cells[x, y] == CellState.ShotMissed;
        }
    }

}
