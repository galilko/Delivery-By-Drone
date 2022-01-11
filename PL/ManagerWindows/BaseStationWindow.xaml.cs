using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class BaseStationWindow : Window, INotifyPropertyChanged
    {
        private IBL bl;

        public event PropertyChangedEventHandler PropertyChanged;
        public static Model Model1 { get; } = Model.Instance;

        BaseStation baseStation;
        public BaseStation BaseStation { get => baseStation; }

        /// <summary>
        /// ctor of add drone window
        /// </summary>
        /// <param name="theBl"></param>
        public BaseStationWindow(BlApi.IBL theBl)
        {
            bl = theBl;
            baseStation = new BaseStation() { Location = new() };
            InitializeComponent();
            this.Width = 400;
            this.Height = 500;
            this.MethodsBSGrid.Visibility = Visibility.Collapsed;
            this.AddBsGrid.Visibility = Visibility.Visible;
           
        }

        public BaseStationWindow(IBL bl, BaseStationToList bstl)
        {
            this.bl = bl;
            baseStation = bl.GetBaseStation(bstl.Id);
            InitializeComponent();
            this.MethodsBSGrid.Visibility = Visibility.Visible;
            this.AddBsGrid.Visibility = Visibility.Collapsed;
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
        private void btnUpdateBS_Click(object sender, RoutedEventArgs e)
        {
            bl.UpdateBaseStation(BaseStation.Id, BSNameTextBox.Text, Convert.ToInt32(SlotsCountTextBox.Text));
            updateBSView();
            MessageBox.Show($"Base Station {BaseStation.Id} was Updated", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            this.UpdateExpander.IsExpanded = false;
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
                this.lvDronesInCharge.Items.Refresh();
            }
        }

        private void btnAddBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.AddBaseStation(BaseStation);
                lock (bl)
                {
                    Model1.BaseStations.Add(bl.GetBSToList(BaseStation.Id));
                }
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

        private void btnDeleteBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete Base-Station?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    BaseStationToList bstl = bl.GetBSToList(BaseStation.Id);
                    bl.DeleteBaseStation((int)BaseStation.Id);
                    lock (bl)
                    {
                        Model1.RemoveBS(bstl);
                    }
                    MessageBox.Show("Base-Station was deleted succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void updateBSView()
        {
            lock (bl)
            {
                baseStation = bl.GetBaseStation((int)BaseStation.Id);
                this.setAndNotify(PropertyChanged, nameof(BaseStation), out baseStation, baseStation);

                BaseStationToList bsToList = Model1.BaseStations.FirstOrDefault(b => b.Id == BaseStation.Id);
                int index = Model1.BaseStations.IndexOf(bsToList);
                if (index >= 0)
                {
                    Model1.BaseStations.Remove(bsToList);
                    Model1.BaseStations.Insert(index, bl.GetBSToList(BaseStation.Id));
                }
            }
        }

    }
}
