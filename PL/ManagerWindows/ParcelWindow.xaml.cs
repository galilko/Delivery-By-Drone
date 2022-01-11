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
using BlApi;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for ParcelWindow.xaml
    /// </summary>
    public partial class ParcelWindow : Window, INotifyPropertyChanged
    {
        private IBL bl;

        public event PropertyChangedEventHandler PropertyChanged;
        public static Model Model1 { get; } = Model.Instance;

        Parcel parcel;
        public Parcel Parcel { get => parcel; }


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
            parcel = new Parcel() { Sender = new(), Target = new() };
            InitializeComponent();
            for (int i = 0; i < bl.GetCustomers().ToList().Count; i++)
            {
                this.cmbSender.Items.Add(bl.GetCustomers().ToList()[i].Id);
                this.cmbTarget.Items.Add(bl.GetCustomers().ToList()[i].Id);
            }
            this.Width = 400;
            this.Height = 500;
            this.MethodsParcelGrid.Visibility = Visibility.Collapsed;
            this.AddParcelGrid.Visibility = Visibility.Visible;
        }

        public ParcelWindow(IBL bl, ParcelToList ptl)
        {
            this.bl = bl;
            parcel = bl.GetParcel(ptl.Id);
            InitializeComponent();
            this.MethodsParcelGrid.Visibility = Visibility.Visible;
            this.AddParcelGrid.Visibility = Visibility.Collapsed;
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

        private void btnAddParcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.AddParcel(Parcel);
                lock (bl)
                {
                    var p = bl.GetParcelToList(bl.GetNextParcelId() - 1);
                    if (Model1.ParcelStatusSelector == null || p.Status == Model1.ParcelStatusSelector)
                        Model1.Parcels.Add(p);
                }
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
            try
            {
                if (MessageBox.Show("Are you sure you want to delete parcel?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ParcelToList ptl = bl.GetParcelToList(Parcel.Id);
                    bl.DeleteParcel(Parcel.Id);
                    lock (bl)
                    {
                        Model1.RemoveParcel(ptl);
                    }
                    MessageBox.Show("Parcel was deleted succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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
        /// <summary>
        /// handle close methods of drone window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void updateCustomersView()
        {
            lock (bl)
            {
                parcel = bl.GetParcel(Parcel.Id);
                this.setAndNotify(PropertyChanged, nameof(Parcel), out parcel, parcel);

                ParcelToList ptl = Model1.Parcels.FirstOrDefault(c => c.Id == Parcel.Id);
                int index = Model1.Parcels.IndexOf(ptl);
                if (index >= 0)
                {
                    Model1.Parcels.Remove(ptl);
                    Model1.Parcels.Insert(index, bl.GetParcelToList(Parcel.Id));
                }
            }
        }

    }
}
