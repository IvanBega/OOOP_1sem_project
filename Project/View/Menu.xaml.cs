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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        public MenuWindow()
        {
            InitializeComponent();
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
    }
}
