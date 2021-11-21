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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        public Settings()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private void Next1_Click(object sender, RoutedEventArgs e)
        {
            // to do: input check
            //ShipArrangement sa = new();
            //sa.Show();
            //this.Close();
            int[] shipCount = new int[5];
            shipCount[0] = int.Parse(PatrolboatCount.Text);
            shipCount[1] = int.Parse(DestroyerCount.Text);
            shipCount[2] = int.Parse(SubmarineCount.Text);
            shipCount[3] = int.Parse(BattleshipCount.Text);
            shipCount[4] = int.Parse(CarrierCount.Text);
            wnd.shipCount = shipCount;
            wnd.SaveAsXmlFormat(shipCount, "ShipCount.xml");
            this.Hide();
            //sa.shipCount = shipCount;
            //sa.SetShipCount();
        }
    }
}
