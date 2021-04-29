using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using Configurate.Managers;
using Configurate.TemplateObjects;

namespace Configurate.Tools
{
    class ApplicationButton
    {
        // VARIABLES
        public readonly Button Button;
        private readonly ApplicationInfoTO myApplication;
        private readonly StackPanel settingsStackPanel;
        private readonly DockPanel topBar;

        // EXECUTION FUNCTIONS
        public ApplicationButton(ApplicationInfoTO app, ref StackPanel stackPanel, ref DockPanel topBar)
        {
            myApplication = app;
            settingsStackPanel = stackPanel;
            this.topBar = topBar;
            Button = UIManager.CreateApplicationButton(myApplication, new RoutedEventHandler(SetUpSettingsObjects));
        }

        // METHODS
        private void SetUpSettingsObjects(object sender, RoutedEventArgs eventArgs)
        {
            // Handle Dirty Applications
            if (ApplicationsManager.IsDirty)
            {
                // Display Dialog Box
                var result = MessageBox.Show("Do you wish to save before closing?", 
                                                "Unsaved Changes", 
                                                MessageBoxButton.YesNoCancel, 
                                                MessageBoxImage.Warning);

                // If the user wants to save, save the current file
                if (result == MessageBoxResult.Yes)
                    SaveCurrentApplication();

                // If the user does not want to save, simply mark the scene as no longer dirty
                else if (result == MessageBoxResult.No)
                    ApplicationsManager.OnDirty?.Invoke(false);

                // If the user wants to cancel, cancel the rest of the process
                else if (result == MessageBoxResult.Cancel)
                    return;
            }

            #region UI Prep
            // Delete all the current settings
            settingsStackPanel.Children.Clear();

            // Get the settings grid
            var scrollView = settingsStackPanel.Parent as ScrollViewer;
            var grid = scrollView.Parent as Grid;

            // Hide the grid
            grid.Visibility = Visibility.Hidden;

            // Change the groupbox title
            var groupBox = grid.Parent as GroupBox;
            groupBox.Header = "Select an Application";
            #endregion

            // Parse settings from the application
            var curfRealDic = new Dictionary<string, string>();
            var parsedSettingsDictionary = FileUtils.ParseCurf(myApplication.CurfPath, 
                                            FileUtils.Parse(myApplication.Path, myApplication.ParserPath), 
                                            ref curfRealDic);

            #region New Application Setup
            // If the parsing process failed, just leave this function
            if (parsedSettingsDictionary == null) return;

            // Setup Display UI
            topBar.Visibility = Visibility.Visible;
            groupBox.Header = myApplication.Name;
            grid.Visibility = Visibility.Visible;

            // Update Application's Manager
            ApplicationsManager.CurrentApplication = myApplication;
            ApplicationsManager.SettingsList = new List<SettingsTO>();
            #endregion

            // Go through all the found settings 
            foreach (var keyPair in parsedSettingsDictionary)
            {
                // Create and setup new settings object
                SettingsTO settingsObj = UIManager.CreateSettingsObject(keyPair.Key, keyPair.Value);
                settingsObj.SetRealPath(curfRealDic[keyPair.Key]);

                // Add setting to the application's manager, for easier saving later
                ApplicationsManager.SettingsList.Add(settingsObj);

                // Update UI
                settingsStackPanel.Children.Add(settingsObj.Grid);
            }
        }

        private void SaveCurrentApplication()
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;
            if (main != null)
            {
                main.SaveCurrentFile();
            }
        }
    }
}
