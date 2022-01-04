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


        private void gif_AnimationCompleted(object sender, RoutedEventArgs e)
        {
            gif.Visibility = Visibility.Collapsed;
            btnAdmin.Visibility = btnUser.Visibility = Visibility.Visible;
            ButtonsBorder.Visibility = Visibility.Visible;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var drone in theBL.AllBlDrones())
            {
                if (drone.Status == BO.DroneStatusCategories.Maintenance)
                    theBL.releaseDrone(drone.Id, new System.TimeSpan(0));
            }
            Close();
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            var lw = new LoginWindow(theBL, true);
            Close();
            lw.ShowDialog();
        }

        private void btnUser_Click(object sender, RoutedEventArgs e)
        {
            var lw = new LoginWindow(theBL, false);
            Close();
            lw.ShowDialog();
        }
    }


}
