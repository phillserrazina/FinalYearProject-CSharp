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

        // CONSTRUCTORS
        public ApplicationInfoTO(string Name, string Path)
        {
            this.Name = Name;
            this.Path = Path;

            CurfPath = $"{Defaults.CURFS}\\{this.Name}.curf";

            // CHANGE THIS TO DEFAULTS' SETUP
            //string iconPath = "/Images/" + Name + "_Logo.png";
            //var iconURI = new Uri(@"pack://application:,,," + iconPath, UriKind.Absolute);
            //var iconURI = new Uri(iconPath, UriKind.RelativeOrAbsolute);
            //Icon = new BitmapImage(iconURI);

            string folderPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Configurate\\Icons\\";
            string iconPath = $"{folderPath}{this.Name}_Logo.png";
            if (!File.Exists(iconPath)) iconPath = $"{folderPath}Unknown.png";
            var iconURI = new Uri(iconPath, UriKind.Absolute);
            Icon = new BitmapImage(iconURI);
        }

        // METHODS
        public void SetPath(string path) => Path = path;
    }
}
