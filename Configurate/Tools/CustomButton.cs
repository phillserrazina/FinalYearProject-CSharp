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
        private readonly ListBox settingsListBox;
        private readonly DockPanel topBar;

        // EXECUTION FUNCTIONS
        public CustomButton(ApplicationInfoTO app, ref ListBox listBox, ref DockPanel topBar)
        {
            myApplication = app;
            settingsListBox = listBox;
            this.topBar = topBar;
            Button = UIManager.CreateApplicationButton(myApplication.Name, myApplication.Icon, new RoutedEventHandler(SetUpLabels));
        }

        // METHODS
        private void SetUpLabels(object sender, RoutedEventArgs eventArgs)
        {
            settingsListBox.Items.Clear();
            var grid = settingsListBox.Parent as Grid;
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
                var settingsObj = UIManager.CreateSettingsObject(keyPair);

                settingsObj.SetRealPath(curfRealDic[keyPair.Key]);

                ApplicationsManager.SettingsList.Add(settingsObj);
                settingsListBox.Items.Add(settingsObj.Grid);
            }
        }
    }
}
