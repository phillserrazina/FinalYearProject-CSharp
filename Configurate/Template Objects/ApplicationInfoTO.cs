using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Configurate.TemplateObjects
{
    class ApplicationInfoTO
    {
        public readonly string Name;
        public string Path { get; private set; }
        public string CurfPath { get; private set; }
        public ImageSource Icon { get; private set; }

        public ApplicationInfoTO(string Name, string Path)
        {
            this.Name = Name;
            this.Path = Path;

            CurfPath = "C:\\Users\\Carolina\\AppData\\Roaming\\Configurate\\CURF Files\\" + this.Name + ".curf";

            // CHANGE THIS TO DEFAULTS' SETUP
            string iconPath = "/Images/" + Name + "_Logo.png";
            var iconURI = new Uri(@"pack://application:,,," + iconPath, UriKind.Absolute);
            //var iconURI = new Uri(iconPath, UriKind.RelativeOrAbsolute);
            Icon = new BitmapImage(iconURI);

            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var iconAppDataPath = System.IO.Path.Combine(appDataPath, "Configurate\\Icons\\" + Name + "_Logo.png");

            if (!File.Exists(iconAppDataPath))
            {
                File.Copy(iconPath, iconAppDataPath);
                
            }
                

            var curfAppDataPath = System.IO.Path.Combine(appDataPath, "Configurate\\CURF Files\\" + Name + ".curf");

            //if (!File.Exists(curfAppDataPath))
            //    File.Copy(iconURI.LocalPath, curfAppDataPath);
        }
    }
}
