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

        public string FinalParserPath { get; private set; }
        public string FinalSaverPath { get; private set; }

        public readonly string Description;
        public readonly string Developer;
        public readonly string Publisher;
        public readonly string ReleaseDate;

        // CONSTRUCTORS
        public ApplicationInfoTO(ApplicationSetupInfoTO setupInfo)
        {
            Name = setupInfo.Name;
            Path = setupInfo.Path;
            ParserPath = setupInfo.ParserFile.Contains(".py") ? $"{Defaults.PARSERS}\\{setupInfo.ParserFile}" : setupInfo.ParserFile;
            SaverPath = setupInfo.SaverFile.Contains(".py") ? $"{Defaults.PARSERS}\\{setupInfo.SaverFile}" : setupInfo.SaverFile;

            FinalParserPath = setupInfo.ParserFile;
            FinalSaverPath = setupInfo.SaverFile;

            Description = setupInfo.Description;
            Developer = setupInfo.Developer;
            Publisher = setupInfo.Publisher;
            ReleaseDate = setupInfo.ReleaseDate;

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
