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
using IBL;
namespace PL
{
    /// <summary>
    /// Interaction logic for Mainwindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IBL.IBL theBL;
        public MainWindow()
        {
            theBL = new IBL.BL();
            InitializeComponent();
        }

        private void btnListDrone_Click(object sender, RoutedEventArgs e)
        {
            new ListDronesWindow(theBL).Show();
        }

        private void btnAddDrone_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(theBL).Show();
        }
    }
}
