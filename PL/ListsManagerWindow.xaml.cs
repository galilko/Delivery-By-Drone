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
    /// Interaction logic for ListsManagerWindow.xaml
    /// </summary>
    public partial class ListsManagerWindow : Window
    {
        BlApi.IBL bl;
        ObservableCollection<DroneToList> obsDronesCollection;
        ObservableCollection<BaseStationToList> obsBSCollection;
        ObservableCollection<CustomerToList> obsCustomersCollection;
        ObservableCollection<ParcelToList> obsParcelsCollection;
        /// <summary>
        ///  ctor of window that present tabs of all listviews
        /// </summary>
        public ListsManagerWindow(BlApi.IBL bl)
        {
            this.bl = bl;
            InitializeComponent();
            this.cmbStatus.ItemsSource = Enum.GetValues(typeof(DroneStatusCategories)); //import cmb options from enum of Statuses
            this.cmbWeight.ItemsSource = Enum.GetValues(typeof(WeightCategories)); //import cmb options from enum of Weights
            this.cmbParcelStatus.ItemsSource = Enum.GetValues(typeof(ParcelStatus)); //import cmb options from enum of Parcel status
            obsDronesCollection = new ObservableCollection<DroneToList>((List<DroneToList>) bl.AllBlDrones());
            this.DronesListView.DataContext = obsDronesCollection; //import all drones to listview
            //obsBSCollection = new ObservableCollection<BaseStationToList>((List<BaseStationToList>) bl.AllBlBaseStations());
            this.BSListView.DataContext = bl.AllBlBaseStations(); //import all base-stations to listview
            obsCustomersCollection = new ObservableCollection<CustomerToList>((List<CustomerToList>) bl.AllBlCustomers());
            this.CustomersListView.DataContext = obsCustomersCollection; //import all customers to listview
            obsParcelsCollection = new ObservableCollection<ParcelToList>((List<ParcelToList>) bl.AllBlParcels());
            this.ParcelsListView.DataContext = obsParcelsCollection; //import all customers to listview

        }


        /// <summary>
        /// handle close window button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            var mw = new MainWindow();
            Close();
            mw.ShowDialog();
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
            
            var dw = new DroneWindow(bl);
            dw.ShowDialog();
            this.DronesListView.Items.Refresh();
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
                var dw = new DroneWindow(bl, dtl);
                dw.ShowDialog();
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
        }

        private void StatusGrouping_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DronesListView.ItemsSource);
            view.GroupDescriptions.Clear();
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Status");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void WeightGrouping_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DronesListView.ItemsSource);
            view.GroupDescriptions.Clear();
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Weight");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void CancelGrouping_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DronesListView.ItemsSource);
            view.GroupDescriptions.Clear();
        }
        #endregion

        #region Base Stations

        private void FreeSlotsGrouping_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(BSListView.ItemsSource);
            view.GroupDescriptions.Clear();
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("FreeChargeSlots");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void CancelBSGrouping_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(BSListView.ItemsSource);
            view.GroupDescriptions.Clear();
        }
        
        private void BSListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = this.BSListView.SelectedItem;
            if (item != null)
            {
                var myItem = item as BaseStationToList;
                BaseStationToList bstl = myItem;
                // send the chosen bs to new methods window
                var bsw = new BaseStationWindow(bl, bstl);
                bsw.ShowDialog();
                this.BSListView.DataContext = bl.AllBlBaseStations(); //import all base-stations to listview
            }
        }

        private void btnAddBaseStation_Click(object sender, RoutedEventArgs e)
        {
            new BaseStationWindow(bl).ShowDialog();
            this.BSListView.DataContext = bl.AllBlBaseStations(); //import all base-stations to listview
        }


        #endregion

        #region Customer
        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl).ShowDialog();
            this.CustomersListView.DataContext = bl.AllBlCustomers(); //import all base-stations to listview
        }

        private void CustomersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = this.CustomersListView.SelectedItem;
            if (item != null)
            {
                var myItem = item as CustomerToList;
                CustomerToList ctl = myItem;
                // send the chosen bs to new methods window
                var cw = new CustomerWindow(bl, ctl);
                cw.ShowDialog();
                this.CustomersListView.DataContext = bl.AllBlCustomers(); //import all base-stations to listview
            }
        }
        #endregion

        #region Parcels
        private void ParcelsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = this.ParcelsListView.SelectedItem;
            if (item != null)
            {
                var myItem = item as ParcelToList;
                ParcelToList ptl = myItem;
                // send the chosen bs to new methods window
                var pw = new ParcelWindow(bl, ptl);
                pw.ShowDialog();
                this.ParcelsListView.DataContext = bl.AllBlParcels(); //import all parcels to listview
            }
        }

        private void SenderGrouping_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelsListView.ItemsSource);
            view.GroupDescriptions.Clear();
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SenderName");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void RecieverGrouping_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelsListView.ItemsSource);
            view.GroupDescriptions.Clear();
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("TargetName");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void CancelParcelGrouping_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelsListView.ItemsSource);
            view.GroupDescriptions.Clear();
        }

        private void cmbParcelStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                if (this.cmbParcelStatus.SelectedItem != null)
                    this.ParcelsListView.ItemsSource = bl.ParcelsByStatus((ParcelStatus)cmbParcelStatus.SelectedItem);
                else
                    this.ParcelsListView.ItemsSource = bl.AllBlParcels();
        }

        private void PStatusGrouping_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelsListView.ItemsSource);
            view.GroupDescriptions.Clear();
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Status");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void btnAddParcel_Click(object sender, RoutedEventArgs e)
        {
            new ParcelWindow(bl).ShowDialog();
            this.ParcelsListView.DataContext = bl.AllBlParcels(); //import all parcels to listview
        }

        private void btnResetParcelStatus_Click(object sender, RoutedEventArgs e)
        {
            cmbParcelStatus.SelectedIndex = -1;
            cmbParcelStatus.Text = "Choose status:";
        }

        #endregion
    }
}
