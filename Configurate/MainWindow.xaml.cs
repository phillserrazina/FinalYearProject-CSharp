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
using System.Diagnostics;

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
            ExportButton.Click += new RoutedEventHandler(ExportCurrentFile);
            ImportButton.Click += new RoutedEventHandler(ImportFile);
            OpenLocationButton.Click += new RoutedEventHandler(OpenCurrentFileAtLocation);
            EditButton.Click += new RoutedEventHandler(EditApplicationLocation);
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

        private void OpenCurrentFileAtLocation(object sender, RoutedEventArgs eventArgs)
        {
            var pathSplit = ApplicationsManager.CurrentApplication.Path.Split('/');

            string finalPath = "";

            for (int i = 0; i < pathSplit.Length - 1; i++)
            {
                finalPath += pathSplit[i] + "/";
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = finalPath,
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }

        private void ExportCurrentFile(object sender, RoutedEventArgs eventArgs)
        {
            var dic = new Dictionary<string, string>();

            foreach (var setting in ApplicationsManager.SettingsList)
            {
                dic.Add(setting.Label.Content.ToString(), setting.Box.Text.ToString());
            }

            bool exported = FileUtils.SaveAs(dic, ApplicationsManager.CurrentApplication.Path);
            if (exported) MessageBox.Show("File Exported Successfuly", "Success");
        }

        private void ImportFile(object sender, RoutedEventArgs eventArgs)
        {

        }

        private void EditApplicationLocation(object sender, RoutedEventArgs eventArgs)
        {
            string newPath = FileUtils.GetNewFilePath(ApplicationsManager.CurrentApplication.Path).Replace('\\', '/');
            ApplicationsManager.CurrentApplication.SetPath(newPath);
            MessageBox.Show(ApplicationsManager.CurrentApplication.Name + "'s Path is now: " + ApplicationsManager.CurrentApplication.Path, "Success");
        }
    }
}
