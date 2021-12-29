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
using BlApi;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for ParcelWindow.xaml
    /// </summary>
    public partial class ParcelWindow : Window
    {
        private IBL bl;
        private ParcelToList ptl;
        private Parcel newP;

        public ParcelWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ctor of add drone window
        /// </summary>
        /// <param name="theBl"></param>
        public ParcelWindow(BlApi.IBL theBl)
        {
            bl = theBl;
            newP = new Parcel();
            InitializeComponent();
            this.cmbWeight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            this.cmbPriority.ItemsSource = Enum.GetValues(typeof(Priorities));
            for (int i = 0; i < bl.AllBlCustomers().ToList().Count; i++)
            {
                this.cmbSender.Items.Add(bl.AllBlCustomers().ToList()[i].Id);
                this.cmbTarget.Items.Add(bl.AllBlCustomers().ToList()[i].Id);
            }
            this.Width = 400;
            this.Height = 500;
            this.MethodsParcelGrid.Visibility = Visibility.Collapsed;
            this.AddParcelGrid.Visibility = Visibility.Visible;
            newP.Sender = new();
            newP.Target = new();
            this.AddParcelGrid.DataContext = newP;
        }

        public ParcelWindow(IBL bl, ParcelToList ptl)
        {
            this.bl = bl;
            this.ptl = ptl;
            InitializeComponent();
            this.MethodsParcelGrid.Visibility = Visibility.Visible;
            this.AddParcelGrid.Visibility = Visibility.Collapsed;
            var p = bl.FindParcel(ptl.Id);
            DataContext = p;
        }


        private void btnAddParcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.AddParcel(newP);
                MessageBox.Show("Parcel was added succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnDeleteParcel_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
