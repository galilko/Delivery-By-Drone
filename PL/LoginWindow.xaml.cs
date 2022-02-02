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
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        BlApi.IBL bl;
        public event PropertyChangedEventHandler PropertyChanged;


        bool isManager;
        public bool IsManager
        {
            get => isManager;
            set => this.setAndNotify(PropertyChanged, nameof(IsManager), out isManager, value);
        }

        bool login;
        public bool Login
        {
            get => login;
            set => this.setAndNotify(PropertyChanged, nameof(Login), out login, value);
        }

        bool register;
        public bool Register
        {
            get => register;
            set => this.setAndNotify(PropertyChanged, nameof(Register), out register, value);
        }

        Customer customer;
        public Customer Customer { get => customer; }

        public LoginWindow(BlApi.IBL bl, bool isManager)
        {
            this.bl = bl;
            IsManager = isManager;
            Login = true;
            Register = false;
            customer = new Customer() { Location = new() };
            InitializeComponent();
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
            if (IsManager)
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
                    BO.Customer loggedCustomer = bl.GetCustomer(customerId);
                    if (loggedCustomer.Id != 0)
                    {
                        var uw = new UserWindow(bl, customerId);
                        Close();
                        uw.ShowDialog();
                    }
                    else
                    {
                        WrongPassword.Text = "Id is incorrect";
                    }
                }
                else
                {
                    WrongPassword.Text = "Id is incorrect";
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
            Close();
            new MainWindow().Show();
        }

        private void SignUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Login)
            {
                Register = true;
                Login = false;
            }
            else
            {
                try
                {
                    AddCustomerGrid.Focus();
                    //Keyboard.Focus(signUplbl);
                    //LongitudeTextBox.Focus();
                    lock (bl)
                    {
                        bl.AddCustomer(Customer);
                    }
                    MessageBox.Show($"Customer {Customer.Id} was added succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    new LoginWindow(bl, false).Show();
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Register = true;
            Login = false;
        }
    }
}
