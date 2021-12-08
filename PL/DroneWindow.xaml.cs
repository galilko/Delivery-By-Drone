using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class DroneWindow : Window
    {
        DroneToList newDrone;
        IBL.IBL bl;
        private DroneToList dtl;

        public DroneWindow(IBL.IBL theBl)
        {
            bl = theBl;
            newDrone = new DroneToList();
            InitializeComponent();
            this.droneGrid.Visibility = Visibility.Visible;
            this.droneGrid.DataContext = newDrone;
            this.cmbWeight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
        }

        public DroneWindow(IBL.IBL theBl, DroneToList myDrone) 
        {
            this.dtl = myDrone;
            this.bl = theBl;
            InitializeComponent();
            this.MethodsDroneGrid.Visibility = Visibility.Visible;
            this.lblDroneDetails.Content = bl.FindDrone(dtl.Id);
            MessageBox.Show(myDrone.ToString());
        }

        private void btnAddDrone_Click(object sender, RoutedEventArgs e)
        {
            bool flag = int.TryParse(this.DroneIdTextBox.Text, out int x);
            if (!flag || this.DroneIdTextBox.Text == null)
            {
                MessageBox.Show($"Invalid Drone Id '{DroneIdTextBox.Text}'", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                DroneIdTextBox.Clear();
                return;
            }
            if (this.DroneModelTextBox.Text == "")
            {
                MessageBox.Show($"You must enter drone model", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (this.cmbWeight.SelectedItem == null)
            {
                MessageBox.Show($"You must choose weight category", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            flag = int.TryParse(this.BusStationTextBox.Text, out int baseStationId);
            if (!flag || this.BusStationTextBox.Text == null)
            {
                MessageBox.Show($"Invalid Base Station '{BusStationTextBox.Text}' Id", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                BusStationTextBox.Clear();
                return;
            }
            try
            {
                bl.AddDrone(newDrone, baseStationId);
                newDrone = new DroneToList();
                this.droneGrid.DataContext = newDrone;
                MessageBox.Show("Drone was added succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnCancelAddDrone_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
        }


        private void DroneIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DroneIdTextBox.Text.Length != 0 && !Regex.IsMatch(DroneIdTextBox.Text, "^[1-9][0-9]{6}$"))
            {
                DroneIdTextBox.Background = Brushes.IndianRed;
                idLabel.Content = "Drone Id must be 7-digits";
            }
            else
            {
                DroneIdTextBox.Background = Brushes.White;
                idLabel.Content = "";
            }
        }

        private void btnUpdateModelToDrone_Click(object sender, RoutedEventArgs e)
        {
            bl.UpdateDroneModel(dtl.Id, NewModelTextBox.Text);
            MessageBox.Show($"Model of Drone {dtl.Id} was changed to '{dtl.Model}'", "Message", MessageBoxButton.OK, MessageBoxImage.Hand);
            this.UpdateExpander.IsExpanded = false;
            this.NewModelTextBox.Text = "";
            this.lblDroneDetails.Content = bl.FindDrone(dtl.Id);
        }

        private void btnChargeDrone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.ChargeDrone(dtl.Id);
                MessageBox.Show($"Drone {dtl.Id} was charged successfully", "Message", MessageBoxButton.OK, MessageBoxImage.Hand);
                this.lblDroneDetails.Content = bl.FindDrone(dtl.Id);
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

        private void btnReleaseDrone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TimeSpan tSpan = new TimeSpan(cmbHours.SelectedIndex, cmbMins.SelectedIndex, 0);
                bl.releaseDrone(dtl.Id, tSpan);
                this.ReleaseExp.IsExpanded = false;
                MessageBox.Show($"Drone {dtl.Id} was released successfully after {tSpan.Hours:00}:{tSpan.Minutes:00} hours", "Message", MessageBoxButton.OK, MessageBoxImage.Hand);
                this.lblDroneDetails.Content = bl.FindDrone(dtl.Id);
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

        private void ChargeReleaseExp_Expanded(object sender, RoutedEventArgs e)
        {
            if (this.dtl.Status == DroneStatusCategories.Free)
                this.btnChargeDrone.Visibility = Visibility.Visible;
            if (this.dtl.Status == DroneStatusCategories.Maintenance)
                this.ReleaseExp.Visibility = Visibility.Visible;
        }
    }
}
