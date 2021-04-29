using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.Generic;

using Configurate.Tools;
using Configurate.Managers;

namespace Configurate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SetupManager setup;

        // CONSTRUCTORS
        public MainWindow()
        {
            InitializeComponent();

            // Run Configurate's Setup (Handles boilerplate work; Mostly setting up files in the AppData Folder)
            setup = new SetupManager();

            // Initialize Events
            ApplicationsManager.OnDirty += OnDirty;
            LoginWindow.OnSuccessfulLogin += OnLogin;
            NewApplicationWindow.OnApplicationsChanged += SetUpApplications;

            // Initialize the Main Window's components
            SetUpApplications();
            SetUpButtons();
        }

        // METHODS
        private void SetUpApplications()
        {
            // Delete all buttons
            ApplicationsStackPanel.Children.Clear();

            // Update setup's applications, in case something has changed
            setup.UpdateApplications();

            // Update the Application's List
            ApplicationsManager.Initialize(setup.ApplicationInfo);

            // Create a Button for each found application
            foreach (var app in ApplicationsManager.ApplicationsList)
            {
                if (File.Exists(app.Path))
                {
                    // Create Button UI
                    var newAppButton = new ApplicationButton(app, ref SettingsStackPanel, ref TopBar);
                    var buttonUI = newAppButton.Button;

                    // Add button to the panel
                    ApplicationsStackPanel.Children.Add(buttonUI);
                }
            }

            // Create "Add New Application" Button
            CreateNewApplicationButton();
        }

        private void SetUpButtons()
        {
            SaveButton.Click += new RoutedEventHandler(SaveFileButton);
            // Enable this if a local database is available
            // ShareButton.Click += new RoutedEventHandler(ShareFile);
        }

        private void CreateNewApplicationButton()
        {
            ApplicationsStackPanel.Children.Add(new StackPanel { Height = 20 });

            var newApplicationButton = new Button
            {
                Height = 30,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = "Add New Application",
                Margin = new Thickness(4),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontWeight = FontWeights.Bold
            };

            newApplicationButton.Click += OpenNewApplicationWindow;

            ApplicationsStackPanel.Children.Add(newApplicationButton);
        }

        #region Button Functions
        private void SaveFileButton(object sender, RoutedEventArgs eventArgs) => SaveCurrentFile();

        private void Autofill(object sender, RoutedEventArgs eventArgs)
        {
            string filePath = $"{Defaults.AUTOFILLS}\\{ApplicationsManager.CurrentApplication.Name}\\";

            // Perform benchmarking process, and select the appropriate file.
            // In a realease scenario, this would be done by running
            // performance tests external to Configurate
            if (Environment.ProcessorCount <= 4) filePath += "Low";
            else if (Environment.ProcessorCount >= 16) filePath += "High";
            else filePath += "Mid";

            filePath += ApplicationsManager.CurrentApplication.Extension;

            // Replace the current file
            ReplaceCurrentFile(filePath);

            // Mark application as dirty
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
                else if (saveSettingsResult == MessageBoxResult.No) ApplicationsManager.OnDirty?.Invoke(false);
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

            bool exported = FileUtils.SaveAs(ApplicationsManager.CurrentApplication.Path);
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

            TriggerLeftWindow("Share");

            UpdateSharePosts();
        }

        private void OpenNewApplicationWindow(object sender, RoutedEventArgs eventArgs)
        {
            NewApplicationWindow newWindow = new NewApplicationWindow();
            newWindow.Show();
            IsHitTestVisible = false;
        }

        public void OnLogin()
        {
            ShareGroupBox.Header = $"Shared { ApplicationsManager.CurrentApplication.Name } Settings";

            Title = $"Configurate ({ NetworkManager.CurrentUser.Username })";

            TriggerLeftWindow("Share");

            UpdateSharePosts();
        }

        private void UpdateSharePosts()
        {
            // Clear the Posts UI
            SharePostsStackPanel.Children.Clear();

            // Get all posts from game
            var (posts, message) = NetworkManager.GetPostsOfGame(ApplicationsManager.CurrentApplication.Name);

            // Go through all the found posts
            foreach (var post in posts)
            {
                // Add a share button 
                SharePostsStackPanel.Children.Add(UIManager.CreateSharedPostButton(post, new RoutedEventHandler((object sender, RoutedEventArgs eventArgs) => {
                    // Get file from temporary server. In a release product, this should fetch a file
                    // from a server stored in a cloud
                    string filePath = $"{Defaults.CONFIGURATE}\\Server\\{post.ID}";
                    filePath += ApplicationsManager.CurrentApplication.Extension;

                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show("File not found on server, please contact an administrator.", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Replace the current file with the fetched file
                    ReplaceCurrentFile(filePath);

                    // Mark Application as dirty
                    ApplicationsManager.OnDirty?.Invoke(true);
                })));
            }
        }

        private void OpenNewPostWindow(object sender, RoutedEventArgs eventArgs)
        {
            TriggerLeftWindow("Post");
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

                TriggerLeftWindow("Share");
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
            // Parse the new file
            var realDic = FileUtils.Parse(newPath, ApplicationsManager.CurrentApplication.ParserPath);
            var curfRealDic = new Dictionary<string, string>();
            var curfDic = FileUtils.ParseCurf(ApplicationsManager.CurrentApplication.CurfPath, realDic, ref curfRealDic);

            // Handle CURF Parsing failure
            if (curfDic == null)
            {
                MessageBox.Show("CURF File Error. Please try again.", "Oops!");
                return;
            }

            // Clear the current settings panel
            SettingsStackPanel.Children.Clear();
            
            // Setup Grid
            var scrollView = SettingsStackPanel.Parent as ScrollViewer;
            var grid = scrollView.Parent as Grid;
            grid.Visibility = Visibility.Hidden;

            // Setup Groupbox
            var groupBox = grid.Parent as GroupBox;
            groupBox.Header = "Select an Application";

            // Final UI Setups
            groupBox.Header = ApplicationsManager.CurrentApplication.Name;
            grid.Visibility = Visibility.Visible;

            // Reset the ApplicationManager's SettingsList
            ApplicationsManager.SettingsList = new List<TemplateObjects.SettingsTO>();

            // Populate the SettingsList with the new values
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
            // Create map that connects the real settings to the displayed label
            var map = new Dictionary<string, string>();

            // Populate the map
            foreach (var setting in ApplicationsManager.SettingsList)
            {
                map.Add(setting.RealPath, setting.Box.Text.ToString());
            }

            // Mark the application as non-dirty anymore
            ApplicationsManager.OnDirty?.Invoke(false);

            // Save the application
            FileUtils.Save(map, 
                            ApplicationsManager.CurrentApplication.Path, 
                            ApplicationsManager.CurrentApplication.SaverPath);
        }

        private void OnDirty(bool isDirty)
        {
            SettingsGroupBox.Header = ApplicationsManager.CurrentApplication.Name + (isDirty ? "*" : "");
        }

        private void OnTextChange(object sender, RoutedEventArgs e)
        {
            string searchParameter = SearchBox.Text.ToLower();
            // Disable all settings that don't match the currently searched parameter
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

        private void OpenAboutWindow(object sender, RoutedEventArgs e)
        {
            var app = ApplicationsManager.CurrentApplication;

            PathText.Text = app.Path;
            ParserText.Text = app.FinalParserPath.Contains(".py") ? $"External ({app.FinalParserPath})" : $"Source ({app.FinalParserPath})";
            SaverText.Text = app.FinalSaverPath.Contains(".py") ? $"External ({app.FinalSaverPath})" : $"Source ({app.FinalSaverPath})";

            DescriptionText.Text = app.Description;
            DeveloperText.Text = app.Developer;
            PublisherText.Text = app.Publisher;
            ReleaseDateText.Text = app.ReleaseDate;

            AppInfoGroupBox.Header = app.Name + " (About)";
            TriggerLeftWindow("Info");
        }

        private void CloseAboutWindow(object sender, RoutedEventArgs e)
        {
            AppInfoGroupBox.Visibility = Visibility.Hidden;
            ApplicationsGroupBox.Visibility = Visibility.Visible;
        }

        private void TriggerLeftWindow(string window)
        {
            AppInfoGroupBox.Visibility = (window == "Info") ? Visibility.Visible : Visibility.Hidden;
            ApplicationsGroupBox.Visibility = (window == "Apps") ? Visibility.Visible : Visibility.Hidden;
            ShareGroupBox.Visibility = (window == "Share") ? Visibility.Visible : Visibility.Hidden;
            NewPostGroupBox.Visibility = (window == "Post") ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
