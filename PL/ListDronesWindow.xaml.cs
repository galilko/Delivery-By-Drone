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
using IBL.BO;
namespace PL
{
    /// <summary>
    /// Interaction logic for ListDronesWindowxaml.xaml
    /// </summary>
    public partial class ListDronesWindow : Window
    {
        IBL.IBL bl;
        public ListDronesWindow(IBL.IBL bl)
        {
            this.bl = bl;

            InitializeComponent();

            this.cmbStatus.ItemsSource = Enum.GetValues(typeof(DroneStatusCategories));
          
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DroneStatusCategories status = (DroneStatusCategories)cmbStatus.SelectedItem;
            this.txtUndefined.Text = status.ToString();

            this.DronesListView.ItemsSource = bl.AllBlDrones(item => item.Status == status);
        }
    }
}
