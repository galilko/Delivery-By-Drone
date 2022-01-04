using BO;
using BlApi;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PL
{
    /// <summary>
    /// Interaction logic for AddDroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        DroneToList newDrone;
        BlApi.IBL bl;
        private DroneToList dtl;
        /// <summary>
        /// ctor of add drone window
        /// </summary>
        /// <param name="theBl"></param>
        public DroneWindow(BlApi.IBL theBl)
        {
            bl = theBl;
            newDrone = new DroneToList();
            InitializeComponent();
            this.Width = 400;
            this.Height = 500;
            this.MethodsDroneGrid.Visibility = Visibility.Collapsed;
            this.AddDroneGrid.Visibility = Visibility.Visible;
            this.AddDroneGrid.DataContext = newDrone;
            this.cmbWeight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            for (int i = 0; i < bl.AllBlBaseStations().ToList().Count; i++)
            {
                this.cmbBaseStation.Items.Add(bl.AllBlBaseStations().ToList()[i].Id);
            }
        }
        /// <summary>
        /// ctor of drone's methods window
        /// </summary>
        /// <param name="theBl"></param>
        /// <param name="myDrone"></param>
        public DroneWindow(BlApi.IBL theBl, DroneToList myDrone)
        {
            this.dtl = myDrone;
            this.bl = theBl;
            InitializeComponent();
            this.MethodsDroneGrid.Visibility = Visibility.Visible;
            this.AddDroneGrid.Visibility = Visibility.Hidden;
            var currentDrone = bl.FindDrone(dtl.Id);
            var currentParcel = currentDrone.CurrentParcel;
            this.MethodsDroneGrid.DataContext = currentDrone;
            if (currentParcel != null)
            {
                this.btnParcel.DataContext = currentParcel;
                btnParcel.IsEnabled = true;
            }
            else
                this.btnParcel.Content = "None";

        }
        /// <summary>
        /// handle adding drone burron click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddDrone_Click(object sender, RoutedEventArgs e)
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
                this.AddDroneGrid.DataContext = newDrone;
                MessageBox.Show("Drone was added succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                var ldw = new ListsManagerWindow(bl);
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
        }
        /// <summary>
        /// handle close add drone window button click 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelAddDrone_Click(object sender, RoutedEventArgs e)
        {
            var ldw = new ListsManagerWindow(bl);
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
            if (DroneIdTextBox.Text.Length != 0 && !Regex.IsMatch(DroneIdTextBox.Text, "^[1-9][0-9]{6}$"))
            {
                DroneIdTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                DroneIdTextBox.BorderBrush = Brushes.Gray;
            }
        }
        /// <summary>
        /// handle update model of drone button-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateModelToDrone_Click(object sender, RoutedEventArgs e)
        {
            bl.UpdateDroneModel(dtl.Id, NewModelTextBox.Text);
            MessageBox.Show($"Model of Drone {dtl.Id} was changed to '{dtl.Model}'", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            this.UpdateExpander.IsExpanded = false;
            this.NewModelTextBox.Text = "";
            this.MethodsDroneGrid.DataContext = bl.FindDrone(dtl.Id);
        }
        /// <summary>
        /// handle charge drone button-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChargeDrone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.ChargeDrone(dtl.Id);
                MessageBox.Show($"Drone {dtl.Id} was charged successfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                this.MethodsDroneGrid.DataContext = bl.FindDrone(dtl.Id);
                this.btnParcel.DataContext = bl.FindDrone(dtl.Id).CurrentParcel;
                this.btnChargeDrone.Visibility = Visibility.Collapsed;
                this.ReleaseExp.Visibility = Visibility.Visible;
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
        /// <summary>
        /// handle charge drone button-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReleaseDrone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TimeSpan tSpan = new TimeSpan(cmbHours.SelectedIndex, cmbMins.SelectedIndex, 0);
                bl.releaseDrone(dtl.Id, tSpan);
                this.ReleaseExp.IsExpanded = false;
                MessageBox.Show($"Drone {dtl.Id} was released successfully after {tSpan.Hours:00}:{tSpan.Minutes:00} hours", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                this.MethodsDroneGrid.DataContext = bl.FindDrone(dtl.Id);
                this.btnParcel.DataContext = bl.FindDrone(dtl.Id).CurrentParcel;
                this.btnChargeDrone.Visibility = Visibility.Visible;
                this.ReleaseExp.Visibility = Visibility.Collapsed;
                this.cmbHours.SelectedIndex = this.cmbMins.SelectedIndex = -1;
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

        private void ReleaseExp_Expanded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i <= 24; i++)
                this.cmbHours.Items.Add($"{ i: 00}");
            for (int i = 0; i <= 60; i++)
                this.cmbMins.Items.Add($"{ i: 00}");
            this.btnReleaseDrone.Visibility = Visibility.Visible;
        }
        private void ReleaseExp_Collapsed(object sender, RoutedEventArgs e)
        {
            this.btnReleaseDrone.Visibility = Visibility.Collapsed;
        }
        private void ChargeReleaseExp_Expanded(object sender, RoutedEventArgs e)
        {
            if (this.dtl.Status == DroneStatusCategories.Free)
                this.btnChargeDrone.Visibility = Visibility.Visible;
            if (this.dtl.Status == DroneStatusCategories.Maintenance)
                this.ReleaseExp.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// handle schedule drone button-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScheduleDrone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.ScheduleDroneForParcel(dtl.Id);
                this.MethodsDroneGrid.DataContext = bl.FindDrone(dtl.Id);
                this.btnParcel.DataContext = bl.FindDrone(dtl.Id).CurrentParcel;
                MessageBox.Show("Drone as Scheduled to parcel succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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
        /// <summary>
        /// handle pick up drone button-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPickudUpDrone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.PickingUpAParcel(dtl.Id);
                this.MethodsDroneGrid.DataContext = bl.FindDrone(dtl.Id);
                this.btnParcel.DataContext = bl.FindDrone(dtl.Id).CurrentParcel;
                MessageBox.Show("Drone Pickud-Up the parcel succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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
        /// <summary>
        /// handle delivered drone button-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeliveredDrone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.DeliverAParcel(dtl.Id);
                this.MethodsDroneGrid.DataContext = bl.FindDrone(dtl.Id);
                this.btnParcel.DataContext = bl.FindDrone(dtl.Id).CurrentParcel;
                MessageBox.Show("Drone Delivered the parcel succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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
            new ParcelDetailsWindow(bl.FindDrone(dtl.Id).CurrentParcel).ShowDialog();
        }

        private void btnDeleteDrone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete Drone?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    bl.DeleteDrone((int)dtl.Id);
                    MessageBox.Show("Drone was deleted succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
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
