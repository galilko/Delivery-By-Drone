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
            gif.Source = new System.Uri(@"C:\Users\Gal Gabay\source\repos\galilko\dotNet5782_6024_2498\PL\giphy.gif");
        }

        private void btnListDrone_Click(object sender, RoutedEventArgs e)
        {
            new ListDronesWindow(theBL).ShowDialog();
        }

        private void btnAddDrone_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(theBL).ShowDialog();
        }
        private void gif_MediaEnded(object sender, RoutedEventArgs e)
        {
            gif.Visibility = Visibility.Collapsed;
            btnAddDrone.Visibility = btnListDrone.Visibility = Visibility.Visible;
            //gif.Position = new System.TimeSpan(0, 0, 1);
            //gif.Play();
        }
    }


}
