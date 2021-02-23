using System;
using System.Collections.Generic;
using System.Text;

namespace Configurate.Tools
{
    class Defaults
    {
        // COMMON SYSTEM LOCATIONS
        public static string DOCUMENTS { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}"; } }
        public static string ROAMING { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}"; } }
        public static string LOCAL { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}"; } }
        public static string LOW { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low"; } }

        // DATA 
        public static string AUTOFILLS { get { return $"{ROAMING}\\Configurate\\Autofills"; } }
        public static string CURFS { get { return $"{ROAMING}\\Configurate\\CURFs"; } }
        public static string ICONS { get { return $"{ROAMING}\\Configurate\\Icons"; } }
        public static string SETUP { get { return $"{ROAMING}\\Configurate\\Setup"; } }
    }
}
