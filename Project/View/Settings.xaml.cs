using Project.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        private int[] shipCount;
        private bool init = false;
        private double difficulty;
        private int tilesLimit = 15;
        private bool validData = false;
        public Settings()
        {
            InitializeComponent();
            this.DataContext = this;
            if (File.Exists("ShipCount.json"))
            {
                shipCount = Serializer.ReadAsJsonFormat <int[]> ("ShipCount.json");
                PatrolboatCount.Text = shipCount[0].ToString();
                DestroyerCount.Text = shipCount[1].ToString();
                SubmarineCount.Text = shipCount[2].ToString();
                BattleshipCount.Text = shipCount[3].ToString();
                CarrierCount.Text = shipCount[4].ToString();
                validData = true;
                wnd.shipCount = shipCount;
                tilesLbl.Content = shipCount[0] + shipCount[1] * 2 + shipCount[2] & 3 + shipCount[3] & 4 + shipCount[4] * 5;
            }
            init = true;
            if (File.Exists("difficulty.json"))
            {
                difficulty = Serializer.ReadAsJsonFormat<double>("difficulty.json");
                dfSlider.Value = difficulty;
                wnd.difficulty = difficulty;
            }        
        }
        private void Next1_Click(object sender, RoutedEventArgs e)
        {  
            if (!validData)
            {
                MessageBox.Show("Incorrect input data!");
                return;
            }
            if (shipCount == null)
            {
                shipCount = new int[5];
            }
            shipCount[0] = int.Parse(PatrolboatCount.Text);
            shipCount[1] = int.Parse(DestroyerCount.Text);
            shipCount[2] = int.Parse(SubmarineCount.Text);
            shipCount[3] = int.Parse(BattleshipCount.Text);
            shipCount[4] = int.Parse(CarrierCount.Text);
            wnd.shipCount = shipCount;
            wnd.difficulty = dfSlider.Value;
            Serializer.SaveAsJsonFormat(shipCount, "ShipCount.json");
            Serializer.SaveAsJsonFormat(difficulty, "difficulty.json");
            this.Hide();
        }
        
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init)
            {
                dfLabel.Content = e.NewValue.ToString("0.00");
            }
        }

        private void Text_Changed(object sender, TextChangedEventArgs e)
        {
            if (!init)
                return;
            bool result1, result2, result3, result4, result5;
            int count1, count2, count3, count4, count5, tiles;
            result1 = int.TryParse(PatrolboatCount.Text, out count1);
            result2 = int.TryParse(DestroyerCount.Text, out count2);
            result3 = int.TryParse(SubmarineCount.Text, out count3);
            result4 = int.TryParse(BattleshipCount.Text, out count4);
            result5 = int.TryParse(CarrierCount.Text, out count5);
            if (result1 && result2 && result3 && result4 && result5
                && count1 >= 0 && count2 >= 0 && count3 >= 0 && count4 >= 0 && count5 >= 0)
            {
                tiles = count1 + 2 * count2 + 3 * count3 + 4 * count4 + 5 * count5;
                tilesLbl.Content = tiles.ToString();
                if (tiles <= tilesLimit)
                {
                    validData = true;
                    tilesLbl.Foreground = Brushes.Black;
                }
                else
                {
                    if (tiles > 99)
                        tilesLbl.Content = "??";
                    validData = false;
                    tilesLbl.Foreground = Brushes.Red;
                }
            }
            else
            {
                tilesLbl.Content = "??";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
