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
    /// Interaction logic for ParcelDetailsWindow.xaml
    /// </summary>
    public partial class ParcelDetailsWindow : Window
    {
        public ParcelDetailsWindow(BO.ParcelInTransfer pit)
        {
            InitializeComponent();
            if (pit != null)
            {
                DataContext = pit;
                lbSenderId.DataContext = lbSenderName.DataContext = pit.Sender;
                lbRecieverId.DataContext = lbRecieverName.DataContext = pit.Reciever;
            }

        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
