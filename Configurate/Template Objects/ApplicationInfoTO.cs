using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Configurate.Tools;

namespace Configurate.TemplateObjects
{
    class ApplicationInfoTO
    {
        // VARIABLES
        public readonly string Name;
        public string Path { get; private set; }
        public string CurfPath { get; private set; }
        public ImageSource Icon { get; private set; }
        public string Extension { get { return System.IO.Path.GetExtension(Path); } }

        // CONSTRUCTORS
        public ApplicationInfoTO(string Name, string Path)
        {
            this.Name = Name;
            this.Path = Path;

            // Get the CURF path
            CurfPath = $"{Defaults.CURFS}\\{this.Name}.curf";

            // CHANGE THIS TO DEFAULTS' SETUP
            //string iconPath = "/Images/" + Name + "_Logo.png";
            //var iconURI = new Uri(@"pack://application:,,," + iconPath, UriKind.Absolute);
            //var iconURI = new Uri(iconPath, UriKind.RelativeOrAbsolute);
            //Icon = new BitmapImage(iconURI);

            // Get the icon path
            string iconPath = $"{Defaults.ICONS}\\{this.Name}_Logo.png";

            // If the icon doesn't exist, switch to the unknown icon
            if (!File.Exists(iconPath)) iconPath = $"{Defaults.ICONS}\\Unknown.png";

            // Convert path to icon
            var iconURI = new Uri(iconPath, UriKind.Absolute);
            Icon = new BitmapImage(iconURI);
        }

        // METHODS
        public void SetPath(string path) => Path = path;
    }
}
