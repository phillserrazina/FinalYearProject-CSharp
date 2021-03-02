using System.Collections.Generic;
using System.Windows;
using Configurate.Managers;
using Configurate.Tools;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System;
using System.Reflection;
using System.Linq;
using System.Management;

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

            //MessageBox.Show($"This computer has {Environment.ProcessorCount} processors.");

            // Run Configurate's Setup (Handles boilerplate work; Mostly setting up files in the AppData Folder)
            SetupManager setup = new SetupManager();

            // Initialize Events
            ApplicationsManager.Initialize(setup.ApplicationInfo);
            ApplicationsManager.OnDirty += OnDirty;
            LoginWindow.OnSuccessfulLogin += OnLogin;

            // Initialize the Main Window's components
            SetUpApplications();
            SetUpButtons();
        }

        // METHODS
        private void SetUpApplications()
        {
            foreach (var app in ApplicationsManager.ApplicationsList)
            {
                if (File.Exists(app.Path))
                    ApplicationsStackPanel.Children.Add(new CustomButton(app, ref SettingsStackPanel, ref TopBar).Button);
            }
        }

        private void SetUpButtons()
        {
            SaveButton.Click += new RoutedEventHandler(SaveFileButton);
            ShareButton.Click += new RoutedEventHandler(ShareFile);
        }

        #region Button Functions
        private void SaveFileButton(object sender, RoutedEventArgs eventArgs) => SaveCurrentFile();

        private void Autofill(object sender, RoutedEventArgs eventArgs)
        {
            uint currentsp, Maxsp;

            using (ManagementObject Mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'"))
            {
                currentsp = (uint)(Mo["CurrentClockSpeed"]);
                Maxsp = (uint)(Mo["MaxClockSpeed"]);
            }

            string filePath = $"{Defaults.AUTOFILLS}\\{ApplicationsManager.CurrentApplication.Name}\\";

            if (Environment.ProcessorCount <= 4) filePath += "Low";
            else if (Environment.ProcessorCount >= 16) filePath += "High";
            else filePath += "Mid";

            filePath += ApplicationsManager.CurrentApplication.Extension;

            ReplaceCurrentFile(filePath);
            ApplicationsManager.OnDirty?.Invoke(true);
        }

        private void HighFill(object sender, RoutedEventArgs eventArgs)
        {
            string filePath = $"{Defaults.AUTOFILLS}\\{ApplicationsManager.CurrentApplication.Name}\\";
            filePath += "High";
            filePath += ApplicationsManager.CurrentApplication.Extension;

            ReplaceCurrentFile(filePath);
            ApplicationsManager.OnDirty?.Invoke(true);
        }

        private void MidFill(object sender, RoutedEventArgs eventArgs)
        {
            string filePath = $"{Defaults.AUTOFILLS}\\{ApplicationsManager.CurrentApplication.Name}\\";
            filePath += "Mid";
            filePath += ApplicationsManager.CurrentApplication.Extension;

            ReplaceCurrentFile(filePath);
            ApplicationsManager.OnDirty?.Invoke(true);
        }

        private void LowFill(object sender, RoutedEventArgs eventArgs)
        {
            string filePath = $"{Defaults.AUTOFILLS}\\{ApplicationsManager.CurrentApplication.Name}\\";
            filePath += "Low";
            filePath += ApplicationsManager.CurrentApplication.Extension;

            ReplaceCurrentFile(filePath);
            ApplicationsManager.OnDirty?.Invoke(true);
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

            UpdateSharePosts();
        }

        public void OnLogin()
        {
            ShareGroupBox.Header = $"Shared { ApplicationsManager.CurrentApplication.Name } Settings";

            Title = $"Configurate ({ NetworkManager.CurrentUser.Username })";

            ApplicationsGroupBox.Visibility = Visibility.Hidden;
            ShareGroupBox.Visibility = Visibility.Visible;

            UpdateSharePosts();
        }

        private void UpdateSharePosts()
        {
            SharePostsStackPanel.Children.Clear();
            //SharePostsStackPanel.Children.Add(UIManager.CreateSharedPostButton("Phill", "This is a test post generated by code.", new RoutedEventHandler((object sender, RoutedEventArgs eventArgs) => { })));
            //SharePostsStackPanel.Children.Add(UIManager.CreateSharedPostButton("Phill Again", "This is another test post generated by code but with more text to see how the boxes handle wrapping.", new RoutedEventHandler((object sender, RoutedEventArgs eventArgs) => { })));

            var (posts, message) = NetworkManager.GetPostsOfGame(ApplicationsManager.CurrentApplication.Name);

            foreach (var post in posts)
            {
                SharePostsStackPanel.Children.Add(UIManager.CreateSharedPostButton(post.Owner, post.Description, new RoutedEventHandler((object sender, RoutedEventArgs eventArgs) => {
                    string filePath = $"{Defaults.CONFIGURATE}\\Server\\{post.ID}";
                    filePath += ApplicationsManager.CurrentApplication.Extension;

                    ReplaceCurrentFile(filePath);
                    ApplicationsManager.OnDirty?.Invoke(true);
                })));
            }
        }

        private void OpenNewPostWindow(object sender, RoutedEventArgs eventArgs)
        {
            ShareGroupBox.Visibility = Visibility.Hidden;
            NewPostGroupBox.Visibility = Visibility.Visible;
        }

        private void CloseNewPostWindow(object sender, RoutedEventArgs eventArgs)
        {
            ShareGroupBox.Visibility = Visibility.Visible;
            NewPostGroupBox.Visibility = Visibility.Hidden;
        }

        private async void CreateNewPost(object sender, RoutedEventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(PostDescriptionTextBox.Text))
            {
                MessageBox.Show("Please add a description to your post.", "Oops!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = await NetworkManager.AddPost(NetworkManager.CurrentUser.Username, PostDescriptionTextBox.Text, ApplicationsManager.CurrentApplication.Name);
            var messageBoxResult = MessageBox.Show(result.Item2, "Result");

            if (messageBoxResult == MessageBoxResult.OK && result.Item1 != null)
            {
                File.Copy(ApplicationsManager.CurrentApplication.Path, $"{Defaults.CONFIGURATE}\\Server\\{result.Item1.ID}{ApplicationsManager.CurrentApplication.Extension}");
                UpdateSharePosts();

                ShareGroupBox.Visibility = Visibility.Visible;
                NewPostGroupBox.Visibility = Visibility.Hidden;
            }
        }

        private void RefreshPosts(object sender, RoutedEventArgs eventArgs) => UpdateSharePosts();

        private void EditApplicationLocation(object sender, RoutedEventArgs eventArgs)
        {
            string fileType = FileUtils.GetFileType(ApplicationsManager.CurrentApplication.Path);
            string windowTitle = "Select New Location";

            string newPath = FileUtils.GetNewFilePath(fileType, windowTitle);

            if (string.IsNullOrEmpty(newPath)) return;

            var importNewPathSettings = MessageBox.Show("Import new location's parameters?", "New Location", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (importNewPathSettings == MessageBoxResult.Yes) ReplaceCurrentFile(newPath);
            else if (importNewPathSettings == MessageBoxResult.Cancel) return;

            ApplicationsManager.OnDirty?.Invoke(true);
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

            SettingsStackPanel.Children.Clear();
            var scrollView = SettingsStackPanel.Parent as ScrollViewer;
            var grid = scrollView.Parent as Grid;
            grid.Visibility = Visibility.Hidden;
            var groupBox = grid.Parent as GroupBox;
            groupBox.Header = "Select an Application";

            groupBox.Header = ApplicationsManager.CurrentApplication.Name;
            grid.Visibility = Visibility.Visible;
            ApplicationsManager.SettingsList = new List<TemplateObjects.SettingsTO>();

            foreach (var keyPair in curfDic)
            {
                var settingsObj = UIManager.CreateSettingsObject(keyPair.Key, keyPair.Value);

                settingsObj.SetRealPath(curfRealDic[keyPair.Key]);

                ApplicationsManager.SettingsList.Add(settingsObj);
                SettingsStackPanel.Children.Add(settingsObj.Grid);
            }
        }

        public void SaveCurrentFile()
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

            ApplicationsManager.OnDirty?.Invoke(true);
            ReplaceCurrentFile(newPath);
        }

        private void OnTextChange(object sender, RoutedEventArgs e)
        {
            string searchParameter = SearchBox.Text.ToLower();
            foreach(var setting in ApplicationsManager.SettingsList)
            {
                setting.SetVisibility(setting.Label.Content.ToString().ToLower().Contains(searchParameter));
            }
        }

        private void CloseShareWindow(object sender, RoutedEventArgs e)
        {
            ShareGroupBox.Visibility = Visibility.Hidden;
            ApplicationsGroupBox.Visibility = Visibility.Visible;
        }
    }
}
