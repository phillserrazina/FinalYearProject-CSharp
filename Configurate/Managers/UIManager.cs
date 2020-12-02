using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using Configurate.TemplateObjects;

namespace Configurate.Managers
{
    class UIManager
    {
        public static SettingsTO CreateSettingsObject(KeyValuePair<string, string> keyPair)
        {
            var cdef1 = new ColumnDefinition { Width = new GridLength(30, GridUnitType.Star) };
            var cdef2 = new ColumnDefinition { Width = new GridLength(70, GridUnitType.Star) };

            var newGrid = new Grid {
                Height = 20,
            };

            newGrid.ColumnDefinitions.Add(cdef1);
            newGrid.ColumnDefinitions.Add(cdef2);

            var newLabel = new Label
            {
                Name = "Label",
                Content = keyPair.Key,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 30,
            };

            var newTextBox = new TextBox
            {
                Name = "TextBox",
                Text = keyPair.Value,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 20
            };

            Grid.SetColumn(newLabel, 0);
            Grid.SetColumn(newTextBox, 1);
            newGrid.Children.Add(newLabel);
            newGrid.Children.Add(newTextBox);

            return new SettingsTO(newGrid, newLabel, newTextBox);
        }

        public static Button CreateApplicationButton(string appName, ImageSource imageSource, RoutedEventHandler buttonEvent)
        {
            var cdef1 = new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) };
            var cdef2 = new ColumnDefinition { Width = new GridLength(90, GridUnitType.Star) };

            var newGrid = new Grid { Focusable = false };

            newGrid.ColumnDefinitions.Add(cdef1);
            newGrid.ColumnDefinitions.Add(cdef2);

            var newImage = new Image
            {
                Name = "Image",
                Margin = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Source = imageSource
            };

            var newLabel = new Label
            {
                Name = "Label",
                Content = appName,
                Width = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 30,
                FontWeight = FontWeights.Bold
            };

            Grid.SetColumn(newImage, 0);
            Grid.SetColumn(newLabel, 1);
            newGrid.Children.Add(newImage);
            newGrid.Children.Add(newLabel);

            var newButton = new Button
            {
                Height = 30,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Content = newGrid
            };

            newButton.Click += buttonEvent;

            return newButton;
        }
    }
}
