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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Configurate.Managers;
using Configurate.Tools;

namespace Configurate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ApplicationsManager.CreateDefault();

            SetUpApplications();
            SetUpButtons();
        }

        private void SetUpApplications()
        {
            foreach (var app in ApplicationsManager.ApplicationsList)
                ApplicationsListBox.Items.Add(new CustomButton(app, ref SettingsListBox).Button);
        }

        private void SetUpButtons()
        {
            SaveButton.Click += new RoutedEventHandler(SaveCurrentFile);
            CloseButton.Click += new RoutedEventHandler(HideSettingsGrid);
        }

        private void SaveCurrentFile(object sender, RoutedEventArgs eventArgs)
        {
            var dic = new Dictionary<string, string>();

            foreach (var setting in ApplicationsManager.SettingsList)
            {
                dic.Add(setting.Label.Content.ToString(), setting.Box.Text.ToString());
            }

            FileUtils.Save(dic, ApplicationsManager.CurrentApplication.Path);
            MessageBox.Show("File Saved Successfuly", "Success");
        }

        private void HideSettingsGrid(object sender, RoutedEventArgs eventArgs)
        {
            SettingsHoldGrid.Visibility = Visibility.Hidden;
            SettingsGroupBox.Header = "Select an Application";
        }
    }
}
