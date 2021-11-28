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
        private AI EnemyAI;
        public double difficulty;
        private Point enemyGridPos = new Point(420,40);
        private int playerCellsLeft;
        private int enemyCellsLeft;
        private bool gameActive = false;
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
            GameBoard.InitGrid(PlayerGrid, playerShips, playerCells, Brushes.Black);
            GameBoard.InitGrid(EnemyGrid, enemyShips, enemyCells, Brushes.Transparent);
            EnemyAI = new(playerCells, difficulty);

            playerCellsLeft = GameBoard.GetOccupiedCells(playerCells);
            enemyCellsLeft = playerCellsLeft;
            botX.Content = playerCellsLeft.ToString();
            botY.Content = enemyCellsLeft.ToString();
            gameActive = true;
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
                if (currentMove == MoveType.PlayerMove)
                    enemyCellsLeft -= 1;
                else
                    playerCellsLeft -= 1;
                botX.Content = playerCellsLeft.ToString();
                botY.Content = enemyCellsLeft.ToString();

                if (damagedShip.DamageCount == damagedShip.Length)
                {
                    GameBoard.DrawShip(damagedShip, opponentGrid, opponentCellState, Brushes.Red, CellState.ShotDestroyedRed); // marking destroyed ship in red
                    GameBoard.DrawShipBorders(damagedShip, opponentGrid, Brushes.White, opponentCellState);
                    if (playerCellsLeft == 0 || enemyCellsLeft == 0)
                    {
                        GameOver();
                        gameActive = false;
                    }
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
                    opponentCellState[pos.X, pos.Y] = CellState.ShotDestroyed;
                }
                //opponentCellState[pos.X, pos.Y] = CellState.ShotDestroyed;
                if (moveType == MoveType.EnemyMove)
                {
                    EnemyAI.AddAdjacentToQueue(pos);
                }
                return true;
            }
            return false;
        }
        private bool ShotCell(CellState[,] cells, int x, int y)
        {
            return cells[x, y] == CellState.ShotMissed || cells[x, y] == CellState.ShotMissed;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settings.Close();
            if (gameActive)
            {
                Serializer.SaveAsJsonFormat<List<Ship>>(playerShips, "playerShips.json");
                Serializer.SaveAsJsonFormat<List<Ship>>(enemyShips, "enemyShips.json");
                Serializer.SaveAsJsonFormat<CellState[,]>(playerCells, "playerCells.json");
                Serializer.SaveAsJsonFormat<CellState[,]>(enemyCells, "enemyCells.json");
                Serializer.SaveAsJsonFormat<MoveType>(currentMove, "currentMove.json");
                Serializer.SaveAsJsonFormat<double>(difficulty, "difficulty.json");
            }
            //Serializer.SaveAsJsonFormat<List<Ship>>(playerShips, "playerShips.json");
            //Serializer.SaveAdList<Ship>(enemyShips, "enemyShips.json");
            
        }
        private async void EnemyAttack()
        {
            bool shootResult = true;
            while (shootResult && gameActive)
            {
                Random r = new();
                int delay = r.Next(900, 2100);
                await Task.Delay(delay);
                Position pos = EnemyAI.PredictMove();
                shootResult = Shoot(pos, MoveType.EnemyMove);
                int x = pos.X + 1;
                int y = pos.Y + 1;
                shotLbl.Content = "Bot shot at: " + x + " " + y;
            }
            currentMove = MoveType.PlayerMove;
        }
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!gameActive)
                return;

            if (currentMove == MoveType.PlayerMove)
            {
                Point pos = e.GetPosition(this);
                int row, column;
                bool shootResult;
                if (pos.X > enemyGridPos.X && pos.X < enemyGridPos.X + 300 && pos.Y > enemyGridPos.Y && pos.Y < enemyGridPos.Y + 300)
                {
                    GameBoard.GetPosByClick(enemyGridPos, pos, out row, out column);

                    if (enemyCells[column, row] == CellState.ShotMissed || enemyCells[column, row] == CellState.ShotDestroyed ||
                        enemyCells[column, row] == CellState.ShotDestroyedRed)
                    {
                        MessageBox.Show("You've already shot this position! Try again...");
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
        public void RestoreGameState()
        {
            gameActive = true;
            playerCells = Serializer.ReadAsJsonFormat<CellState[,]>("playerCells.json");
            enemyCells = Serializer.ReadAsJsonFormat<CellState[,]>("enemyCells.json");
            playerShips = Serializer.ReadAsJsonFormat<List<Ship>>("playerShips.json");
            enemyShips = Serializer.ReadAsJsonFormat<List<Ship>>("enemyShips.json");
            currentMove = Serializer.ReadAsJsonFormat<MoveType>("currentMove.json");
            GameBoard.DrawCellsOnGrid(PlayerGrid, playerCells, true);
            GameBoard.DrawCellsOnGrid(EnemyGrid, enemyCells, false);
            foreach(Ship s in playerShips)
            {
                GameBoard.DrawShipBorders(s, PlayerGrid, Brushes.White, playerCells);
            }
            foreach(Ship s in enemyShips)
            {
                GameBoard.DrawShipBorders(s, EnemyGrid, Brushes.White, enemyCells);
            }
            EnemyAI = new AI(playerCells, difficulty);
            playerCellsLeft = GameBoard.GetOccupiedCells(playerCells);
            enemyCellsLeft = GameBoard.GetOccupiedCells(enemyCells);
            botX.Content = playerCellsLeft.ToString();
            botY.Content = enemyCellsLeft.ToString();
            this.Show();
            if (currentMove == MoveType.EnemyMove)
            {
                EnemyAttack();
            }
        }
        private void GameOver()
        {
            DeleteAllFiles();
            GameBoard.DrawCellsOnGrid(EnemyGrid, enemyCells, true);
            foreach(Ship s in enemyShips)
            {
                GameBoard.DrawShipBorders(s, EnemyGrid, Brushes.White, enemyCells);
            }
            if (currentMove == MoveType.PlayerMove)
            {
                MessageBox.Show("Congratulations! You have won the game!");
            }
            else
            {
                MessageBox.Show("Oh no! It looks like you have lost...");
            }
            
        }
        private void DeleteAllFiles()
        {
            if (File.Exists("playerCells.json"))
                File.Delete("playerCells.json");

            if (File.Exists("enemyCells.json"))
                File.Delete("enemyCells.json");

            if (File.Exists("playerShips.json"))
                File.Delete("playerShips.json");

            if (File.Exists("enemyShips.json"))
                File.Delete("enemyShips.json");

            if (File.Exists("currentMove.json"))
                File.Delete("currentMove.json");
        }
    }

}
