using System.Windows;
namespace PL
{
    /// <summary>
    /// Interaction logic for Mainwindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BlApi.IBL theBL;
        public MainWindow()
        {
            theBL = BlApi.BlFactory.GetBL();
            InitializeComponent();
        }

        private void btnListDrone_Click(object sender, RoutedEventArgs e)
        {
            var lw = new LoginWindow(theBL);
            Close();
            lw.ShowDialog();
        }

        private void btnAddDrone_Click(object sender, RoutedEventArgs e)
        {
            Close();
            new DroneWindow(theBL).ShowDialog();
        }

        private void gif_AnimationCompleted(object sender, RoutedEventArgs e)
        {
            gif.Visibility = Visibility.Collapsed;
            btnAddDrone.Visibility = btnListDrone.Visibility = Visibility.Visible;
            ButtonsBorder.Visibility = Visibility.Visible;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }


}
