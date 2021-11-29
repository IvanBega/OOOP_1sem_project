using System.IO;
using System.Windows;

namespace Project.View
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private readonly MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        private bool shipArraInitialized = false;
        public MenuWindow()
        {
            InitializeComponent();
            if (File.Exists("playerCells.json") && File.Exists("enemyCells.json") && File.Exists("currentMove.json")
                && File.Exists("playerShips.json") && File.Exists("enemyShips.json"))
            {
                ContinueBtn.IsEnabled = true;
            }
        }

        private void NewGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!shipArraInitialized)
            {
                wnd.shipArrangement = new();
                shipArraInitialized = true;
            }
            wnd.shipArrangement.SetShipCount();
            wnd.shipArrangement.Show();
            Hide();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            wnd.settings.Show();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            wnd.Close();
            if (wnd.shipArrangement != null)
                wnd.shipArrangement.Close();
        }

        private void ContinueBtn_Click(object sender, RoutedEventArgs e)
        {
            wnd.RestoreGameState();
            this.Close();
        }
    }
}
