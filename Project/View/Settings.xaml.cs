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
                //
                wnd.shipCount = shipCount;
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
            // to do: input check
            //ShipArrangement sa = new();
            //sa.Show();
            //this.Close();   
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
            //sa.shipCount = shipCount;
            //sa.SetShipCount();
        }
        
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init)
            {
                dfLabel.Content = e.NewValue.ToString("0.00");
            }
        }
    }
}
