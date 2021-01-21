using System.Collections.Generic;
using System.Windows;
using Configurate.Managers;
using Configurate.Tools;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace Configurate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // CONSTRUCTORS
        public MainWindow()
        {
            InitializeComponent();

            ApplicationsManager.OnDirty += OnDirty;
            LoginWindow.OnSuccessfulLogin += OnLogin;

            SetUpApplications();
            SetUpButtons();
        }

        // METHODS
        private void SetUpApplications()
        {
            foreach (var app in ApplicationsManager.ApplicationsList)
            {
                if (File.Exists(app.Path))
                    ApplicationsListBox.Items.Add(new CustomButton(app, ref SettingsListBox, ref TopBar).Button);
            }
        }

        private void SetUpButtons()
        {
            SaveButton.Click += new RoutedEventHandler(SaveFileButton);
            ShareButton.Click += new RoutedEventHandler(ShareFile);
        }

        #region Button Functions
        private void SaveFileButton(object sender, RoutedEventArgs eventArgs)
        {
            SaveCurrentFile();
        }

        private void HideSettingsGrid(object sender, RoutedEventArgs eventArgs)
        {
            if (ApplicationsManager.IsDirty)
            {
                var saveSettingsResult = MessageBox.Show("Do you wish to save before closing?", "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (saveSettingsResult == MessageBoxResult.Yes) SaveCurrentFile();
                else if (saveSettingsResult == MessageBoxResult.Cancel) return;
            }

            TopBar.Visibility = Visibility.Collapsed;

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
            if (exported) MessageBox.Show("File Exported Successfuly.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShareFile(object sender, RoutedEventArgs eventArgs)
        {
            if (!NetworkManager.LoggedIn)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                IsHitTestVisible = false;

                return;
            }

            ShareGroupBox.Header = $"Shared { ApplicationsManager.CurrentApplication.Name } Settings";

            ApplicationsGroupBox.Visibility = Visibility.Hidden;
            ShareGroupBox.Visibility = Visibility.Visible;
        }

        public void OnLogin()
        {
            ShareGroupBox.Header = $"Shared { ApplicationsManager.CurrentApplication.Name } Settings";

            Title = $"Configurate ({ NetworkManager.CurrentUser.Username })";

            ApplicationsGroupBox.Visibility = Visibility.Hidden;
            ShareGroupBox.Visibility = Visibility.Visible;
        }

        private void EditApplicationLocation(object sender, RoutedEventArgs eventArgs)
        {
            string fileType = FileUtils.GetFileType(ApplicationsManager.CurrentApplication.Path);
            string windowTitle = "Select New Location";

            string newPath = FileUtils.GetNewFilePath(fileType, windowTitle);

            if (string.IsNullOrEmpty(newPath)) return;

            var importNewPathSettings = MessageBox.Show("Import new location's parameters?", "New Location", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (importNewPathSettings == MessageBoxResult.Yes) ReplaceCurrentFile(newPath);
            else if (importNewPathSettings == MessageBoxResult.Cancel) return;

            newPath = newPath.Replace('\\', '/');
            ApplicationsManager.CurrentApplication.SetPath(newPath);

            MessageBox.Show(ApplicationsManager.CurrentApplication.Name + "'s Path is now: " + ApplicationsManager.CurrentApplication.Path, "Success");
        }
        #endregion

        private void ReplaceCurrentFile(string newPath)
        {
            var realDic = FileUtils.Parse(newPath);
            var curfRealDic = new Dictionary<string, string>();
            var curfDic = FileUtils.ParseCurf(ApplicationsManager.CurrentApplication.CurfPath, realDic, ref curfRealDic);
            if (curfDic == null)
            {
                MessageBox.Show("CURF File Error. Please try again.", "Oops!");
                return;
            }

            SettingsListBox.Items.Clear();
            var grid = SettingsListBox.Parent as Grid;
            var groupBox = grid.Parent as GroupBox;

            groupBox.Header = ApplicationsManager.CurrentApplication.Name;
            grid.Visibility = Visibility.Visible;
            ApplicationsManager.SettingsList = new List<TemplateObjects.SettingsTO>();

            foreach (var keyPair in curfDic)
            {
                var settingsObj = UIManager.CreateSettingsObject(keyPair);

                settingsObj.SetRealPath(curfRealDic[keyPair.Key]);

                ApplicationsManager.SettingsList.Add(settingsObj);
                SettingsListBox.Items.Add(settingsObj.Grid);
            }

            ApplicationsManager.OnDirty?.Invoke(false);
        }

        private void SaveCurrentFile()
        {
            var dic = new Dictionary<string, string>();

            foreach (var setting in ApplicationsManager.SettingsList)
            {
                dic.Add(setting.RealPath, setting.Box.Text.ToString());
            }

            ApplicationsManager.OnDirty?.Invoke(false);

            FileUtils.Save(dic, ApplicationsManager.CurrentApplication.Path);
            MessageBox.Show("File Saved Successfuly", "Success");
        }

        private void OnDirty(bool isDirty)
        {
            SettingsGroupBox.Header = ApplicationsManager.CurrentApplication.Name + (isDirty ? "*" : "");
        }

        private void ImportFileManually()
        {
            string fileType = FileUtils.GetFileType(ApplicationsManager.CurrentApplication.Path);
            string windowTitle = "Import File";

            string newPath = FileUtils.GetNewFilePath(fileType, windowTitle).Replace('\\', '/');

            if (string.IsNullOrEmpty(newPath))
            {
                MessageBox.Show("Couldn't Import File. Please try again.", "Oops!");
                return;
            }

            ReplaceCurrentFile(newPath);
        }

        private void OnTextChange(object sender, RoutedEventArgs e)
        {
            string searchParameter = SearchBox.Text;
            foreach(var setting in ApplicationsManager.SettingsList)
            {
                setting.SetVisibility(setting.Label.Content.ToString().Contains(searchParameter));
            }
        }
    }
}
