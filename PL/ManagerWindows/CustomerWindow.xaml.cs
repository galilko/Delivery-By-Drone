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
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window, INotifyPropertyChanged
    {
        private IBL bl;

        public event PropertyChangedEventHandler PropertyChanged;
        public static Model Model1 { get; } = Model.Instance;

        Customer customer;
        public Customer Customer { get => customer; }

        public CustomerWindow(IBL bl)
        {
            this.bl = bl;
            customer = new Customer() { Location = new() };
            InitializeComponent();
            this.Width = 400;
            this.Height = 500;
            this.MethodsCustomerGrid.Visibility = Visibility.Collapsed;
            this.AddCustomerGrid.Visibility = Visibility.Visible;
        }

        public CustomerWindow(IBL bl, CustomerToList ctl)
        {
            this.bl = bl;
            customer = bl.GetCustomer((int)ctl.Id);
            InitializeComponent();
            this.MethodsCustomerGrid.Visibility = Visibility.Visible;
            this.AddCustomerGrid.Visibility = Visibility.Collapsed;
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
                bl.AddCustomer(Customer);
                lock (bl)
                {
                    Model1.Customers.Add(bl.GetCustomerToList((int)Customer.Id));
                }
                MessageBox.Show($"Customer {Customer.Id} was added succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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
            bl.UpdateCustomer((int)Customer.Id, Customer.Name, Customer.PhoneNumber);
            updateCustomersView();
            MessageBox.Show($"Customer {Customer.Id} was Updated", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            this.UpdateExpander.IsExpanded = false;
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

        private void btnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete Customer?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    CustomerToList ctl = bl.GetCustomerToList((int)Customer.Id);
                    bl.DeleteCustomer((int)Customer.Id);
                    lock (bl)
                    {
                        Model1.RemoveCustomer(ctl);
                    }
                    MessageBox.Show("Customer was deleted succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void updateCustomersView()
        {
            lock (bl)
            {
                customer = bl.GetCustomer((int)Customer.Id);
                this.setAndNotify(PropertyChanged, nameof(Customer), out customer, customer);

                CustomerToList ctl = Model1.Customers.FirstOrDefault(c => c.Id == Customer.Id);
                int index = Model1.Customers.IndexOf(ctl);
                if (index >= 0)
                {
                    Model1.Customers.Remove(ctl);
                    Model1.Customers.Insert(index, bl.GetCustomerToList((int)Customer.Id));
                }
            }
        }

    }
}
