using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;
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
        public int[] shipCount { get; set; }
        public List<Ship> playerShips { get; set; } = new();
        public List<Ship> enemyShips { get; set; }
        public CellState[,] playerCells { get; set; } = new CellState[10, 10];
        public CellState[,] enemyCells { get; set; } = new CellState[10, 10];
        public Settings settings;
        private MenuWindow menu;
        private MoveType currentMove = MoveType.PlayerMove;
        public event Action FinishedMove;
        private AI EnemyAI;
        private Point enemyGridPos = new Point(420,40);

        public void RestoreGameState()
        {
            playerCells = Serializer.ReadAsJsonFormat<CellState[,]>("playerCells.json");
            enemyCells = Serializer.ReadAsJsonFormat<CellState[,]>("enemyCells.json");
            playerShips = Serializer.ReadAsJsonFormat<List<Ship>>("playerShips.json");
            enemyShips = Serializer.ReadAsJsonFormat<List<Ship>>("enemyShips.json");
            currentMove = Serializer.ReadAsJsonFormat<MoveType>("currentMove.json");

            GameBoard.InitGrid(PlayerGrid, playerShips, playerCells);
            GameBoard.InitGrid(EnemyGrid, enemyShips, enemyCells);
            GameBoard.DrawCellsOnGrid(PlayerGrid, playerCells);
            GameBoard.DrawCellsOnGrid(EnemyGrid, enemyCells);

            EnemyAI = new AI(playerCells);
            this.Show();
        }

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            settings = new();
            menu = new();
            Hide();
            menu.Show();
            //s.Show();
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
            SoundEffect.PlayShootSound();
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
        private bool ShotCell(CellState[,] cells, int x, int y)
        {
            return cells[x, y] == CellState.ShotMissed || cells[x, y] == CellState.ShotMissed;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settings.Close();
            Serializer.SaveAsJsonFormat<List<Ship>>(playerShips, "playerShips.json");
            Serializer.SaveAsJsonFormat <List<Ship>>(enemyShips, "enemyShips.json");
            Serializer.SaveAsJsonFormat<CellState[,]>(playerCells, "playerCells.json");
            Serializer.SaveAsJsonFormat<CellState[,]>(enemyCells, "enemyCells.json");
            Serializer.SaveAsJsonFormat<MoveType>(currentMove, "currentMove.json");
            //Serializer.SaveAsJsonFormat<List<Ship>>(playerShips, "playerShips.json");
            //Serializer.SaveAdList<Ship>(enemyShips, "enemyShips.json");
            
        }
        private async void EnemyAttack()
        {
            bool shootResult = true;
            while (shootResult)
            {
                Random r = new();
                int delay = r.Next(900, 2100);
                await Task.Delay(delay);
                Position pos = EnemyAI.PredictMove();
                shootResult = Shoot(pos, MoveType.EnemyMove);
            }
            currentMove = MoveType.PlayerMove;
        }
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentMove == MoveType.PlayerMove)
            {
                Point pos = e.GetPosition(this);
                int row, column;
                bool shootResult;
                if (pos.X > enemyGridPos.X && pos.X < enemyGridPos.X + 300 && pos.Y > enemyGridPos.Y && pos.Y < enemyGridPos.Y + 300)
                {
                    GameBoard.GetPosByClick(enemyGridPos, pos, out row, out column);

                    if (enemyCells[column, row] == CellState.ShotMissed || enemyCells[column, row] == CellState.ShotDestroyed)
                    {
                        MessageBox.Show("You've already shot this position! Try again...");
                        btn1.Visibility = Visibility.Visible;
                        return;
                    }
                    shootResult = Shoot(new Position(column, row), MoveType.PlayerMove);
                    if (!shootResult)
                    {
                        currentMove = MoveType.EnemyMove;
                        EnemyAttack();
                    }
                }
            }
        }
    }

}
