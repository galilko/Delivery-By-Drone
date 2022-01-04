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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        BlApi.IBL bl;
        bool isManager;
        public LoginWindow(BlApi.IBL bl, bool isManager)
        {
            this.bl = bl;
            this.isManager = isManager;
            InitializeComponent();
            PasswordPanel.Visibility = isManager? Visibility.Visible : Visibility.Collapsed;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (isManager)
            {
                if (UserNameTextBox.Text == "admin" && PasswordBox.Password == "123")
                {
                    var ldw = new ListsManagerWindow(bl);
                    Close();
                    ldw.ShowDialog();
                }
                else
                {
                    WrongPassword.Text = "username or password are incorrect";
                }
            }
            else
            {
                if (int.TryParse(UserNameTextBox.Text, out int customerId))
                {
                    BO.Customer loggedCustomer = bl.FindCustomer(customerId);
                    if (!loggedCustomer.Equals(default(BO.Customer)))
                    {
                        var uw = new UserWindow(bl, customerId);
                        Close();
                        uw.ShowDialog();
                    }
                }
            }
        }

        private void UserNameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            WrongPassword.Text = "";
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            WrongPassword.Text = "";
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            var mw = new MainWindow();
            Close();
            mw.ShowDialog();
        }
    }
}
