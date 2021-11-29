using Project.Model;
using Project.Model.Ships;
using Project.View;
using Project.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int[] ShipCount { get; set; }
        public List<Ship> PlayerShips { get; set; } = new();
        public List<Ship> EnemyShips { get; set; }
        public CellState[,] PlayerCells { get; set; } = new CellState[10, 10];
        public CellState[,] EnemyCells { get; set; } = new CellState[10, 10];
        public Settings settings;
        public ShipArrangement shipArrangement;
        public MenuWindow menu;
        private MoveType currentMove = MoveType.PlayerMove;
        private AI EnemyAI;
        public double difficulty;
        private Point enemyGridPos = new(420, 40);
        private int playerCellsLeft;
        private int enemyCellsLeft;
        private bool gameActive = false;
        public bool ShipArrangementInitialized = false;
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
            GameBoard.InitGrid(PlayerGrid, PlayerShips, PlayerCells, Brushes.Black);
            GameBoard.InitGrid(EnemyGrid, EnemyShips, EnemyCells, Brushes.Transparent);
            EnemyAI = new(PlayerCells, difficulty);

            playerCellsLeft = GameBoard.GetOccupiedCells(PlayerCells);
            enemyCellsLeft = GameBoard.GetOccupiedCells(EnemyCells);
            botX.Content = playerCellsLeft.ToString();
            botY.Content = enemyCellsLeft.ToString();
            gameActive = true;
        }
        public bool Shoot(Position pos, MoveType moveType)
        {
            SoundEffect.PlayShootSound();
            CellState[,] opponentCellState = PlayerCells;
            List<Ship> opponentShips = PlayerShips;
            Grid opponentGrid = PlayerGrid;
            if (moveType == MoveType.PlayerMove)
            {
                opponentCellState = EnemyCells;
                opponentShips = EnemyShips;
                opponentGrid = EnemyGrid;
            }

            if (opponentCellState[pos.X, pos.Y] == CellState.Free)
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
                opponentCellState[pos.X, pos.Y] = CellState.ShotMissed;
            }

            if (opponentCellState[pos.X, pos.Y] == CellState.Occupied)
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
        //private bool ShotCell(CellState[,] cells, int x, int y)
        //{
        //    return cells[x, y] == CellState.ShotMissed || cells[x, y] == CellState.ShotMissed;
        //}
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settings.Close();
            menu.Close();
            if (gameActive)
            {
                Serializer.SaveAsJsonFormat<List<Ship>>(PlayerShips, "playerShips.json");
                Serializer.SaveAsJsonFormat<List<Ship>>(EnemyShips, "enemyShips.json");
                Serializer.SaveAsJsonFormat<CellState[,]>(PlayerCells, "playerCells.json");
                Serializer.SaveAsJsonFormat<CellState[,]>(EnemyCells, "enemyCells.json");
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
                bool shootResult;
                if (pos.X > enemyGridPos.X && pos.X < enemyGridPos.X + 300 && pos.Y > enemyGridPos.Y && pos.Y < enemyGridPos.Y + 300)
                {
                    GameBoard.GetPosByClick(enemyGridPos, pos, out int row, out int column);

                    if (EnemyCells[column, row] == CellState.ShotMissed || EnemyCells[column, row] == CellState.ShotDestroyed ||
                        EnemyCells[column, row] == CellState.ShotDestroyedRed)
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
            PlayerCells = Serializer.ReadAsJsonFormat<CellState[,]>("playerCells.json");
            EnemyCells = Serializer.ReadAsJsonFormat<CellState[,]>("enemyCells.json");
            PlayerShips = Serializer.ReadAsJsonFormat<List<Ship>>("playerShips.json");
            EnemyShips = Serializer.ReadAsJsonFormat<List<Ship>>("enemyShips.json");
            currentMove = Serializer.ReadAsJsonFormat<MoveType>("currentMove.json");
            GameBoard.DrawCellsOnGrid(PlayerGrid, PlayerCells, true);
            GameBoard.DrawCellsOnGrid(EnemyGrid, EnemyCells, false);
            foreach (Ship s in PlayerShips)
            {
                GameBoard.DrawShipBorders(s, PlayerGrid, Brushes.White, PlayerCells);
            }
            foreach (Ship s in EnemyShips)
            {
                GameBoard.DrawShipBorders(s, EnemyGrid, Brushes.White, EnemyCells);
            }
            EnemyAI = new AI(PlayerCells, difficulty);
            playerCellsLeft = GameBoard.GetOccupiedCells(PlayerCells);
            enemyCellsLeft = GameBoard.GetOccupiedCells(EnemyCells);
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
            GameBoard.DrawCellsOnGrid(EnemyGrid, EnemyCells, true);
            foreach (Ship s in EnemyShips)
            {
                GameBoard.DrawShipBorders(s, EnemyGrid, Brushes.White, EnemyCells);
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
