using System.Windows.Controls;

namespace Configurate.TemplateObjects
{
    class SettingsTO
    {
        public Grid Grid { get; private set; }
        public Label Label { get; private set; }
        public TextBox Box { get; private set; }

        public SettingsTO(Grid Grid, Label Label, TextBox Box)
        {
            this.Grid = Grid;
            this.Label = Label;
            this.Box = Box;
        }
    }
}
