using System.Windows;
using System.ComponentModel;

using Configurate.Managers;

namespace Configurate
{
    /// <summary>
    /// Lógica interna para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public delegate void OnSuccessfulLoginDelegate();
        public static OnSuccessfulLoginDelegate OnSuccessfulLogin;

        public LoginWindow()
        {
            InitializeComponent();

            // Automatically Populate the fields for easier testing
            UsernameTextBox.Text = "PhillAdmin";
            PasswordTextBox.Password = "admin";
        }

        public void OnWindowExit(object sender, CancelEventArgs e)
        {
            Application.Current.MainWindow.IsHitTestVisible = true;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate login
            var (result, resultMessage) = NetworkManager.GetUser(UsernameTextBox.Text, PasswordTextBox.Password);

            // Handle login success
            if (result != null)
            {
                NetworkManager.LogIn(result);
                OnSuccessfulLogin?.Invoke();

                Close();
            }

            // Handle login failure
            else
            {
                MessageBox.Show(resultMessage, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
