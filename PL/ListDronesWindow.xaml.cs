using BO;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Data;

namespace PL
{
    /// <summary>
    /// Interaction logic for ListDronesWindowxaml.xaml
    /// </summary>
    public partial class ListDronesWindow : Window
    {
        BlApi.IBL bl;
        ObservableCollection<DroneToList> obsDronesCollection;
        ObservableCollection<BaseStationToList> obsBSCollection;
        ObservableCollection<CustomerToList> obsCustomersCollection;
        ObservableCollection<ParcelToList> obsParcelsCollection;
        /// <summary>
        ///  ctor of window that present listview of all drones
        /// </summary>
        public ListDronesWindow(BlApi.IBL bl)
        {
            this.bl = bl;
            InitializeComponent();
            this.cmbStatus.ItemsSource = Enum.GetValues(typeof(DroneStatusCategories)); //import cmb options from enum of Statuses
            this.cmbWeight.ItemsSource = Enum.GetValues(typeof(WeightCategories)); //import cmb options from enum of Weights
            obsDronesCollection = new ObservableCollection<DroneToList>((List<DroneToList>) bl.AllBlDrones());
            this.DronesListView.DataContext = obsDronesCollection; //import all drones to listview
            obsBSCollection = new ObservableCollection<BaseStationToList>((List<BaseStationToList>) bl.AllBlBaseStations());
            this.BSListView.DataContext = obsBSCollection; //import all base-stations to listview
            obsCustomersCollection = new ObservableCollection<CustomerToList>((List<CustomerToList>) bl.AllBlCustomers());
            this.CustomersListView.DataContext = obsCustomersCollection; //import all customers to listview
            obsParcelsCollection = new ObservableCollection<ParcelToList>((List<ParcelToList>) bl.AllBlParcels());
            this.ParcelsListView.DataContext = obsParcelsCollection; //import all customers to listview

        }

        #region Drones
        /// <summary>
        /// update the drones list view according to combobox of statuses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbStatus.SelectedIndex == -1)
            {
                if (this.cmbWeight.SelectedItem != null)
                    this.DronesListView.ItemsSource = bl.AllBlDrones(item => item.Weight == (WeightCategories)cmbWeight.SelectedItem);
                else
                    this.DronesListView.ItemsSource = bl.AllBlDrones();
                return;
            }
            DroneStatusCategories status = (DroneStatusCategories)cmbStatus.SelectedItem;
            this.txtStatusSort.Text = status.ToString();
            if (this.cmbWeight.SelectedItem != null)
                this.DronesListView.ItemsSource = bl.AllBlDrones(item => item.Status == status && item.Weight == (WeightCategories)cmbWeight.SelectedItem);
            else
                this.DronesListView.ItemsSource = bl.AllBlDrones(item => item.Status == status);
        }
        /// <summary>
        /// update the drones list view according to combobox of weights
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbWeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbWeight.SelectedIndex == -1)
            {
                if (this.cmbStatus.SelectedItem != null)
                    this.DronesListView.ItemsSource = bl.AllBlDrones(item => item.Status == (DroneStatusCategories)cmbStatus.SelectedItem);
                else
                    this.DronesListView.ItemsSource = bl.AllBlDrones();
                return;
            }
            WeightCategories weight = (WeightCategories)cmbWeight.SelectedItem;
            this.txtWeightSort.Text = weight.ToString();
            if (this.cmbStatus.SelectedItem != null)
                this.DronesListView.ItemsSource = bl.AllBlDrones(item => item.Weight == weight && item.Status == (DroneStatusCategories)cmbStatus.SelectedItem);
            else
                this.DronesListView.ItemsSource = bl.AllBlDrones(item => item.Weight == weight);
        }
        /// <summary>
        /// handle of openning "add drone" window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddDrone_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).ShowDialog();
            obsDronesCollection = new ObservableCollection<DroneToList>((List<DroneToList>)bl.AllBlDrones()); //import all drones to listview
            DronesListView.DataContext = obsDronesCollection;
        }
        /// <summary>
            /// handle event of double-click on some drone
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
        private void DronesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = this.DronesListView.SelectedItem;
            if (item != null)
            {
                var myItem = item as DroneToList;
                DroneToList dtl = myItem;
                // send the chosen drone to new methods window
                new DroneWindow(bl, dtl).ShowDialog();
                this.DronesListView.Items.Refresh();
            }
        }
        /// <summary>
        /// handle click on reset weight sort
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetWeight_Click(object sender, RoutedEventArgs e)
        {
            cmbWeight.SelectedIndex = -1;
            cmbWeight.Text = "Choose weight:";
            txtWeightSort.Text = "";
        }
        /// <summary>
        /// handle click on reset status sort
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetStatus_Click(object sender, RoutedEventArgs e)
        {
            cmbStatus.SelectedIndex = -1;
            cmbStatus.Text = "Choose weight:";
            txtStatusSort.Text = "";
        }
        /// <summary>
        /// handle close window button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DronesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion


    }
}
