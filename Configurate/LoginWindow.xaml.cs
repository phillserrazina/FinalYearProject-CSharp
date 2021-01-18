using Configurate.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

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
        }

        public void OnWindowExit(object sender, CancelEventArgs e)
        {
            Application.Current.MainWindow.IsHitTestVisible = true;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var (result, resultMessage) = NetworkManager.GetUser(UsernameTextBox.Text, PasswordTextBox.Password);

            if (result != null)
            {
                NetworkManager.LogIn(result);
                OnSuccessfulLogin?.Invoke();

                Close();
            }
            else
            {
                MessageBox.Show(resultMessage, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
