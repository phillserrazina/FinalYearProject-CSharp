using System;
using System.Collections.Generic;
using System.Text;

namespace Configurate.Tools
{
    class Defaults
    {
        public static string DOCUMENTS { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}"; } }
        public static string ROAMING { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}"; } }
        public static string LOCAL { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}"; } }
        public static string LOW { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low"; } }
        public static string CURFS { get { return $"{ROAMING}\\Configurate\\CURFs";  } }
    }
}
