using System.Windows.Controls;
using Configurate.Managers;
using Configurate.TemplateObjects;
using System.Windows;
using System.Collections.Generic;

namespace Configurate.Tools
{
    class CustomButton
    {
        // VARIABLES
        public readonly Button Button;
        private readonly ApplicationInfoTO myApplication;
        private readonly StackPanel settingsStackPanel;
        private readonly DockPanel topBar;

        // EXECUTION FUNCTIONS
        public CustomButton(ApplicationInfoTO app, ref StackPanel stackPanel, ref DockPanel topBar)
        {
            myApplication = app;
            settingsStackPanel = stackPanel;
            this.topBar = topBar;
            Button = UIManager.CreateApplicationButton(myApplication.Name, myApplication.Icon, new RoutedEventHandler(SetUpSettingsObjects));
        }

        // METHODS
        private void SetUpSettingsObjects(object sender, RoutedEventArgs eventArgs)
        {
            if (ApplicationsManager.IsDirty)
            {
                var saveSettingsResult = MessageBox.Show("Do you wish to save before closing?", "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (saveSettingsResult == MessageBoxResult.Yes)
                {
                    // SHOULD NOT BE DOING THIS!!!
                    var main = Application.Current.MainWindow as MainWindow;
                    if (main != null)
                    {
                        main.SaveCurrentFile();
                    }
                }
                else if (saveSettingsResult == MessageBoxResult.Cancel) return;
            }

            settingsStackPanel.Children.Clear();
            var scrollView = settingsStackPanel.Parent as ScrollViewer;
            var grid = scrollView.Parent as Grid;
            grid.Visibility = Visibility.Hidden;
            var groupBox = grid.Parent as GroupBox;
            groupBox.Header = "Select an Application";

            var curfRealDic = new Dictionary<string, string>();
            var dic = FileUtils.ParseCurf(myApplication.CurfPath, FileUtils.Parse(myApplication.Path), ref curfRealDic);
            if (dic == null) return;

            topBar.Visibility = Visibility.Visible;

            groupBox.Header = myApplication.Name;
            grid.Visibility = Visibility.Visible;
            ApplicationsManager.CurrentApplication = myApplication;
            ApplicationsManager.SettingsList = new List<SettingsTO>();

            foreach (var keyPair in dic)
            {
                var settingsObj = UIManager.CreateSettingsObject(keyPair.Key, keyPair.Value);

                settingsObj.SetRealPath(curfRealDic[keyPair.Key]);

                ApplicationsManager.SettingsList.Add(settingsObj);
                settingsStackPanel.Children.Add(settingsObj.Grid);
            }
        }
    }
}
