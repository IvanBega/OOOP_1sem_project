﻿using System;
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
        public int BattleshipCount { get; set; } = 1;
        public int CarrierCount { get; set; } = 1;
        public int DestroyerCount { get; set; } = 1;
        public int PatrolBoatCount { get; set; } = 1;
        public int SubmarineCount { get; set; } = 1;
        public Settings()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Next1_Click(object sender, RoutedEventArgs e)
        {
            ShipArrangement sa = new();
            sa.Show();
            this.Close();
            sa.PassShipCount(this);       
        }
    }
}