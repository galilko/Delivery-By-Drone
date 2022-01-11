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
        public static Model Model { get; } = Model.Instance;
        

        /// <summary>
        ///  ctor of window that present tabs of all listviews
        /// </summary>
        public ListsManagerWindow(BlApi.IBL bl)
        {
            this.bl = bl;
            InitializeComponent();

        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Model.DronesRefresh();
            Model.BaseStationsRefresh();
            Model.CustomersRefresh();
            Model.ParcelsRefresh();
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
       /* /// <summary>
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
        }*/
        /// <summary>
        /// handle of openning "add drone" window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddDrone_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).ShowDialog();
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
                Window_Loaded(sender, e);
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
                Window_Loaded(sender, e);
            }
        }

        private void btnAddBaseStation_Click(object sender, RoutedEventArgs e)
        {
            new BaseStationWindow(bl).ShowDialog();
        }


        #endregion

        #region Customer
        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl).ShowDialog();
        }

        private void CustomersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = this.CustomersListView.SelectedItem;
            if (item != null)
            {
                var myItem = item as CustomerToList;
                CustomerToList ctl = myItem;
                var cw = new CustomerWindow(bl, ctl);
                cw.ShowDialog();
                Window_Loaded(sender, e);
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
                new ParcelWindow(bl, ptl).ShowDialog();
                Window_Loaded(sender, e);
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
        }

        private void btnResetParcelStatus_Click(object sender, RoutedEventArgs e)
        {
            cmbParcelStatus.SelectedIndex = -1;
            cmbParcelStatus.Text = "Choose status:";
        }

        #endregion

        private void btnParcelsByDate_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan newTime = new TimeSpan(24, 0, 0);
            
            this.ParcelsListView.ItemsSource = bl.ParcelsByDates(fromDate.SelectedDate , toDate.SelectedDate + newTime);
        }

        private void btnResetByDate_Click(object sender, RoutedEventArgs e)
        {
            fromDate.SelectedDate = toDate.SelectedDate = null;
            this.ParcelsListView.ItemsSource = bl.GetParcels();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
        }
    }
}
