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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        public MenuWindow()
        {
            InitializeComponent();
            if (File.Exists("playerCells.json") && File.Exists("enemyCells.json") && File.Exists("currentMove.json") 
                && File.Exists("playerShips.json") && File.Exists("EnemyShips.json"))
            {
                ContinueBtn.IsEnabled = true;
            }
        }

        private void NewGameBtn_Click(object sender, RoutedEventArgs e)
        {
            ShipArrangement sa = new();
            sa.Show();
            sa.SetShipCount();
            Close();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            wnd.settings.Show();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            wnd.Close();
        }

        private void ContinueBtn_Click(object sender, RoutedEventArgs e)
        {
            wnd.RestoreGameState();
            this.Close();
        }
    }
}
