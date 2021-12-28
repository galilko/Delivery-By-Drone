using BlApi;
using BO;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for BaseStationWindow.xaml
    /// </summary>
    public partial class BaseStationWindow : Window
    {
        private IBL bl;
        private BaseStationToList bstl;
        private BaseStation newBS;

        public BaseStationWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ctor of add drone window
        /// </summary>
        /// <param name="theBl"></param>
        public BaseStationWindow(BlApi.IBL theBl)
        {
            bl = theBl;
            newBS = new BaseStation();
            InitializeComponent();
            this.Width = 400;
            this.Height = 500;
            this.MethodsBSGrid.Visibility = Visibility.Collapsed;
            this.AddBsGrid.Visibility = Visibility.Visible;
            this.AddBsGrid.DataContext = newBS;
            newBS.BSLocation = new();
            LongitudeTextBox.DataContext = LatitudeIdTextBox.DataContext = newBS.BSLocation;
        }

        public BaseStationWindow(IBL bl, BaseStationToList bstl)
        {
            this.bl = bl;
            this.bstl = bstl;
            InitializeComponent();
            this.MethodsBSGrid.Visibility = Visibility.Visible;
            this.AddBsGrid.Visibility = Visibility.Collapsed;
            var bs = bl.FindBaseStation(bstl.Id);
            DataContext = bs;
            lvDronesInCharge.DataContext = bs.DronesInCharge;
        }



        /// <summary>
        /// handle adding drone burron click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       /* private void btnAddDrone_Click(object sender, RoutedEventArgs e)
        {
            bool flag = int.TryParse(this.DroneIdTextBox.Text, out int x);
            if (!flag || !Regex.IsMatch(DroneIdTextBox.Text, "^[1-9][0-9]{6}$")) //handle invalid input to drone id
            {
                MessageBox.Show($"Invalid Drone Id '{DroneIdTextBox.Text}'", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                DroneIdTextBox.Clear();
                return;
            }
            if (this.DroneModelTextBox.Text == "") //handle invalid input to drone model
            {
                MessageBox.Show($"You must enter drone model", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (this.cmbWeight.SelectedItem == null) //handle invalid input to drone weight
            {
                MessageBox.Show($"You must choose weight category", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (this.cmbBaseStation.SelectedItem == null) //handle invalid input to base station
            {
                MessageBox.Show($"You must choose Base Station", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                int baseStationId = (int)this.cmbBaseStation.SelectedItem;
                bl.AddDrone(newDrone, baseStationId);
                newDrone = new DroneToList();
                this.AddBsGrid.DataContext = newDrone;
                MessageBox.Show("Drone was added succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                var ldw = new ListDronesWindow(bl);
                Close();
                ldw.ShowDialog();
            }
            catch (Exception ex)
            {
                string msg = $"{ex.Message}\n";
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    msg += $"{ex.Message}\n";
                }

                MessageBox.Show(msg, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }*/
        /// <summary>
        /// handle close add drone window button click 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelAddDrone_Click(object sender, RoutedEventArgs e)
        {
            var ldw = new ListDronesWindow(bl);
            Close();
            ldw.ShowDialog();
        }
        /// <summary>
        /// handle design of drone id text box if is invalid input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DroneIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           /* if (DroneIdTextBox.Text.Length != 0 && !Regex.IsMatch(DroneIdTextBox.Text, "^[1-9][0-9]{6}$"))
            {
                DroneIdTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                DroneIdTextBox.BorderBrush = Brushes.Gray;
            }*/
        }
        /// <summary>
        /// handle update model of drone button-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateModelToDrone_Click(object sender, RoutedEventArgs e)
        {
            bl.UpdateBaseStation(bstl.Id, BSNameTextBox.Text, Convert.ToInt32(SlotsCountTextBox.Text));
            MessageBox.Show($"Base Station {bstl.Id} was Updated", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            this.UpdateExpander.IsExpanded = false;
            var bs = bl.FindBaseStation(bstl.Id);
            DataContext = bs;
        }

        
    
        
        /// <summary>
        /// handle close methods of drone window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Map_OnClick(object sender, RoutedEventArgs e)
        {
            /*
            var longitude = dtl.CurrentLocation.Longitude;
            var latitude = dtl.CurrentLocation.Latitude;
            try
            {
                var googleMapsAddress = $"https://www.google.co.il//maps/@{longitude},{latitude},18z?hl=iw";


                //var bingMapsAddress = $"https://www.bing.com/maps?cp={longitude}~{latitude}&lvl=18";

                ShowMap.Source = new Uri(googleMapsAddress);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't load the map of station! \n" + ex.Message, "map Loading Error!");
            }*/
        }

        private void btnParcel_Click(object sender, RoutedEventArgs e)
        {
        }

        private void lvDronesInCharge_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = this.lvDronesInCharge.SelectedItem;
            if (item != null)
            {
                var myItem = item as DroneInCharge;
                DroneToList dtl = new() { Id = myItem.Id };
                // send the chosen bs to new methods window
                var dw = new DroneWindow(bl, dtl);
                dw.ShowDialog();
                //this.DronesListView.Items.Refresh();
            }
        }

        private void btnAddBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.AddBaseStation(newBS);
                MessageBox.Show("Base Station was added succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                string msg = $"{ex.Message}\n";
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    msg += $"{ex.Message}\n";
                }

                MessageBox.Show(msg, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
