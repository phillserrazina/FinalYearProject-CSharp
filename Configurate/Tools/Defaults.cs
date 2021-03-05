using System;

namespace Configurate.Tools
{
    class Defaults
    {
        // COMMON SYSTEM LOCATIONS
        public static string DOCUMENTS { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}"; } }
        public static string ROAMING { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}"; } }
        public static string LOCAL { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}"; } }
        public static string LOW { get { return $"{LOCAL}Low"; } }

        // DATA 
        public static string CONFIGURATE { get { return $"{ROAMING}\\Configurate"; } }
        public static string AUTOFILLS { get { return $"{CONFIGURATE}\\Autofills"; } }
        public static string PARSERS { get { return $"{CONFIGURATE}\\Parsers"; } }
        public static string CURFS { get { return $"{CONFIGURATE}\\CURFs"; } }
        public static string ICONS { get { return $"{CONFIGURATE}\\Icons"; } }
        public static string SETUP { get { return $"{CONFIGURATE}\\Setup"; } }
    }
}
