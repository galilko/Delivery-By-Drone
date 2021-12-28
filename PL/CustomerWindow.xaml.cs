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
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        private IBL bl;
        private CustomerToList ctl;
        private Customer newC;

        public CustomerWindow()
        {
            InitializeComponent();
        }

        public CustomerWindow(IBL bl)
        {
            this.bl = bl;
            newC = new Customer();
            InitializeComponent();
            this.Width = 400;
            this.Height = 500;
            this.MethodsBSGrid.Visibility = Visibility.Collapsed;
            this.AddCustomerGrid.Visibility = Visibility.Visible;
            this.AddCustomerGrid.DataContext = newC;
            newC.CustomerLocation = new();
            LongitudeTextBox.DataContext = LatitudeIdTextBox.DataContext = newC.CustomerLocation;

        }

        public CustomerWindow(IBL bl, CustomerToList ctl)
        {
            this.bl = bl;
            this.ctl = ctl;
            InitializeComponent();
            this.MethodsBSGrid.Visibility = Visibility.Visible;
            this.AddCustomerGrid.Visibility = Visibility.Collapsed;
            var c = bl.FindCustomer(ctl.Id);
            DataContext = c;
            lvParcelSent.DataContext = c.ParcelFromCustomerList;
            lvParcelRecieved.DataContext = c.ParcelToCustomerList;
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

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.AddCustomer(newC);
                MessageBox.Show($"Customer {newC.Id} was added succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnUpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            bl.UpdateCustomer(ctl.Id, CNameTextBox.Text, CPhoneTextBox.Text);
            MessageBox.Show($"Customer {ctl.Id} was Updated", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            this.UpdateExpander.IsExpanded = false;
            var c = bl.FindCustomer(ctl.Id);
            DataContext = c;
        }

        private void lvParcelSent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = this.lvParcelSent.SelectedItem;
            if (item != null)
            {
                var myItem = item as ParcelAtCustomer;
                Parcel p = new() { Id = myItem.Id };
                // send the chosen bs to new methods window
                //var dw = new ParcelWindow(bl, p);
                //dw.ShowDialog();
            }
        }

        private void lvParcelRecieved_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = this.lvParcelRecieved.SelectedItem;
            if (item != null)
            {
                var myItem = item as ParcelAtCustomer;
                Parcel p = new() { Id = myItem.Id };
                // send the chosen bs to new methods window
                //var dw = new ParcelWindow(bl, p);
                //dw.ShowDialog();
            }
        }
    }
}
