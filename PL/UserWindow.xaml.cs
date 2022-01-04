using BlApi;
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
using BO;
namespace PL
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        BlApi.IBL bl;
        int customerId;
        Parcel newP;
        public UserWindow()
        {
            InitializeComponent();
        }

        public UserWindow(IBL bl, int customerId)
        {
            this.bl = bl;
            this.customerId = customerId;
            newP = new Parcel();
            InitializeComponent();
            ParcelsSentListView.DataContext = bl.GetSentParcels(customerId);
            ParcelsRecievedListView.DataContext = bl.GetRecievedParcels(customerId);
            this.cmbWeight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            this.cmbPriority.ItemsSource = Enum.GetValues(typeof(Priorities));

            /*for (int i = 0; i < bl.AllBlCustomers().ToList().Count; i++)
            {
                if(bl.AllBlCustomers().ToList()[i].Id != customerId)
                    this.cmbTarget.Items.Add(bl.AllBlCustomers().ToList()[i].Id);
            }*/
            List<BO.CustomerToList> CustomersList = bl.AllBlCustomers().ToList();
            cmbTarget.ItemsSource = CustomersList;
            this.txtSender.Text = customerId.ToString();
            newP.Sender = new() { Id = customerId };
            newP.Target = new();
            SendParcelGrid.DataContext = newP;
        }

        private void btnSendParcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.AddParcel(newP);
                MessageBox.Show("Parcel was sent succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                ParcelsSentListView.DataContext = bl.GetSentParcels(customerId);
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

        private void cmbTarget_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            newP.Target.Id = (int)cmbTarget.SelectedValue;

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

    }
}
