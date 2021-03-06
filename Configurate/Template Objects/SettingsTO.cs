﻿using System.Windows.Controls;

namespace Configurate.TemplateObjects
{
    class SettingsTO
    {
        // VARIABLES
        public Grid Grid { get; private set; }
        public Label Label { get; private set; }
        public string RealPath { get; private set; }
        public TextBox Box { get; private set; }

        // CONSTRUCTOR
        public SettingsTO(Grid Grid, Label Label, TextBox Box)
        {
            this.Grid = Grid;
            this.Label = Label;
            this.Box = Box;
        }

        // METHODS
        public void SetRealPath(string s) => RealPath = s;
        public void SetVisibility(bool val) => Grid.Visibility = val ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }
}
