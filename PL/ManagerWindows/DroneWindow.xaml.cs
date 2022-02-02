using BO;
using BlApi;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for AddDroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window, INotifyPropertyChanged
    {
        BlApi.IBL bl;

        public event PropertyChangedEventHandler PropertyChanged;
        public static Model Model1 { get; } = Model.Instance;

        Drone drone;
        public Drone Drone { get => drone; }

        DroneToList dtl;
        public DroneToList NewDrone { get => dtl; }

        bool auto;
        public bool Auto
        {
            get => auto;
            set => this.setAndNotify(PropertyChanged, nameof(Auto), out auto, value);
        }

        /// <summary>
        /// ctor of add drone window
        /// </summary>
        /// <param name="theBl"></param>
        public DroneWindow(BlApi.IBL theBl)
        {
            bl = theBl;
            dtl = new DroneToList();
            InitializeComponent();
            this.Width = 400;
            this.Height = 500;
            this.MethodsDroneGrid.Visibility = Visibility.Collapsed;
            this.AddDroneGrid.Visibility = Visibility.Visible;
            for (int i = 0; i < bl.GetBaseStations().ToList().Count; i++)
            {
                this.cmbBaseStation.Items.Add(bl.GetBaseStations().ToList()[i].Id);
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
            drone = bl.GetDrone((int)myDrone.Id);
            InitializeComponent();
            this.MethodsDroneGrid.Visibility = Visibility.Visible;
            this.AddDroneGrid.Visibility = Visibility.Hidden;
            var currentDrone = bl.GetDrone((int)dtl.Id);
            var currentParcel = currentDrone.CurrentParcel;
            auto = false;
            //this.MethodsDroneGrid.DataContext = currentDrone;
            /*if (currentParcel != null)
            {
                this.btnParcel.DataContext = currentParcel;
                btnParcel.IsEnabled = true;
            }
            else
                this.btnParcel.Content = "None";*/

        }
        /// <summary>
        /// make it possible to drag the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
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
                bl.AddDrone(NewDrone, baseStationId);
                lock (bl)
                {
                    var drone = bl.GetDroneToList((int)NewDrone.Id);
                    if ((Model1.StatusSelector == null || drone.Status == Model1.StatusSelector) &&
                        (Model1.WeightSelector == null || drone.Weight == Model1.WeightSelector))
                        Model1.Drones.Add(drone);
                }
                MessageBox.Show("Drone was added succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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
            bl.UpdateDroneModel((int)dtl.Id, NewModelTextBox.Text);
            updateDroneView();
            MessageBox.Show($"Model of Drone {dtl.Id} was changed to '{dtl.Model}'", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            this.UpdateExpander.IsExpanded = false;         
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
                bl.ChargeDrone((int)dtl.Id);
                updateDroneView();
                MessageBox.Show($"Drone {dtl.Id} was charged successfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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
                bl.releaseDrone((int)dtl.Id);
                updateDroneView();
                TimeSpan tSpan = DateTime.Now - dtl.StartCharge;
                MessageBox.Show($"Drone {dtl.Id} was released successfully after {tSpan.Hours:00}:{tSpan.Minutes:00} hours", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                //this.MethodsDroneGrid.DataContext = bl.FindDrone(dtl.Id);
                //this.btnParcel.DataContext = bl.FindDrone(dtl.Id).CurrentParcel;
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
        /// handle schedule drone button-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScheduleDrone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.ScheduleDroneForParcel((int)dtl.Id);
                updateDroneView();
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
                bl.PickingUpAParcel((int)dtl.Id);
                updateDroneView();
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
                bl.DeliverAParcel((int)dtl.Id);
                updateDroneView();
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
            var longitude = dtl.Location.Longitude;
            var latitude = dtl.Location.Latitude;
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
            new ParcelDetailsWindow(bl.GetDrone((int)dtl.Id).CurrentParcel).ShowDialog();
        }

        private void btnDeleteDrone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete Drone?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    bl.DeleteDrone((int)dtl.Id);
                    lock (bl)
                    {
                        Model1.RemoveDrone(Drone);
                    }
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

        private void updateDroneView()
        {
            lock (bl)
            {
                drone = bl.GetDrone((int)Drone.Id);
                this.setAndNotify(PropertyChanged, nameof(Drone), out drone, drone);

                DroneToList droneToList = Model1.Drones.FirstOrDefault(d => d.Id == Drone.Id);
                int index = Model1.Drones.IndexOf(droneToList);
                if (index >= 0)
                {
                    Model1.Drones.Remove(droneToList);
                    Model1.Drones.Insert(index, bl.GetDroneToList((int)Drone.Id));
                }
            }
        }


        BackgroundWorker worker;
        private void updateDrone() => worker.ReportProgress(0);
        private bool checkStop() => worker.CancellationPending;

        private void Auto_Click(object sender, RoutedEventArgs e)
        {
            Auto = true;
            worker = new() { WorkerReportsProgress = true, WorkerSupportsCancellation = true, };
            worker.DoWork += (sender, args) => bl.StartDroneSimulator((int)args.Argument, updateDrone, checkStop);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                Auto = false;
                worker = null;
               // if (closing) Close();
            };
            worker.ProgressChanged += (sender, args) => updateDroneView();
            worker.RunWorkerAsync(Drone.Id);
        }

        private void Manual_Click(object sender, RoutedEventArgs e) => worker?.CancelAsync();

    }
}
