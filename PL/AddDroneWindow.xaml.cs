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
    /// Interaction logic for AddDroneWindow.xaml
    /// </summary>
    public partial class AddDroneWindow : Window
    {
        DroneToList newDrone;
        IBL.IBL bl;
        public AddDroneWindow(IBL.IBL theBl)
        {
            bl = theBl;

            newDrone = new DroneToList();
            this.DataContext = newDrone;

            InitializeComponent();

        //    this.Weight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
        }
    }
}
