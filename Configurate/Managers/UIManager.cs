﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using Configurate.TemplateObjects;

namespace Configurate.Managers
{
    class UIManager
    {
        // METHODS
        public static SettingsTO CreateSettingsObject(string settingName, Dictionary<string, string> settingValues)
        {
            // Grid Columns
            var cdef1 = new ColumnDefinition { Width = new GridLength(30, GridUnitType.Star) };
            var cdef2 = new ColumnDefinition { Width = new GridLength(70, GridUnitType.Star) };

            // Create Grid
            var newGrid = new Grid {
                Height = 20,
                Margin = new Thickness(0, 2, 5, 2)
            };

            // Add columns to grid
            newGrid.ColumnDefinitions.Add(cdef1);
            newGrid.ColumnDefinitions.Add(cdef2);

            // Create label
            var newLabel = new Label
            {
                Name = "Label",
                Content = settingName,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 30
            };

            // Create tooltip
            var tooltipStackPanel = new StackPanel();
            var tooltipTitle = new Label { Content = newLabel.Content, FontWeight = FontWeights.Bold };

            string description = settingValues.ContainsKey("Description") ? settingValues["Description"] : "No Description Available.";

            var tooltipDescription = new Label { Content = description };

            tooltipStackPanel.Children.Add(tooltipTitle);
            tooltipStackPanel.Children.Add(tooltipDescription);

            var labelTooltip = new ToolTip { Content = tooltipStackPanel };
            newLabel.ToolTip = labelTooltip;

            // Create input field
            var newTextBox = new TextBox
            {
                Name = "TextBox",
                Text = settingValues["Value"],
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 20
            };

            newTextBox.TextChanged += TextChangedEventHandler;

            // Add label and input field to grid
            Grid.SetColumn(newLabel, 0);
            Grid.SetColumn(newTextBox, 1);
            newGrid.Children.Add(newLabel);
            newGrid.Children.Add(newTextBox);

            // Create and return a new settings object
            return new SettingsTO(newGrid, newLabel, newTextBox);
        }

        private static void TextChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            ApplicationsManager.OnDirty?.Invoke(true);
        }

        public static Button CreateApplicationButton(ApplicationInfoTO app, RoutedEventHandler buttonEvent)
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
                Source = app.Icon
            };

            var newLabel = new Label
            {
                Name = "Label",
                Content = app.Name,
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
                Content = newGrid,
                Margin = new Thickness(2)
            };

            newButton.Click += buttonEvent;

            return newButton;
        }

        public static Button CreateSharedPostButton(string postOwner, string postContent, RoutedEventHandler buttonEvent)
        {
            var dockPanel = new DockPanel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = double.NaN,
                Height = double.NaN
            };

            var ownerLabel = new Label
            {
                Name = "Label",
                Content = postOwner,
                Width = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            };

            var descriptionText = new TextBlock
            {
                Text = postContent,
                Margin = new Thickness(5, 0, 5, 0),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var ratingLabel = new Label
            {
                Name = "Label",
                Content = "Rating: *****",
                Width = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            dockPanel.Children.Add(ownerLabel);
            dockPanel.Children.Add(descriptionText);
            dockPanel.Children.Add(ratingLabel);

            DockPanel.SetDock(ownerLabel, Dock.Top);
            DockPanel.SetDock(descriptionText, Dock.Top);
            DockPanel.SetDock(ratingLabel, Dock.Bottom);

            var button = new Button
            {
                Margin = new Thickness(2),
                Content = dockPanel,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch
            };

            button.Click += buttonEvent;

            return button;
        }
    }
}
