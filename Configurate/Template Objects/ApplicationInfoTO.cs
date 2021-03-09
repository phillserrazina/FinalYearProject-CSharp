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
        public string CurfPath { get { return $"{Defaults.CURFS}\\{Name}.curf"; } }
        public string ParserPath { get; private set; }
        public string SaverPath { get; private set; }
        public readonly ImageSource Icon;
        public string Extension { get { return FileUtils.GetFileType(Path); } }

        // CONSTRUCTORS
        public ApplicationInfoTO(string Name, string Path, string ParserPath, string SaverPath)
        {
            this.Name = Name;
            this.Path = Path;
            this.ParserPath = $"{Defaults.PARSERS}\\{ParserPath}";
            this.SaverPath = $"{Defaults.PARSERS}\\{SaverPath}";

            // Get the icon path
            string iconPath = $"{Defaults.ICONS}\\{Name}_Logo.png";

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
