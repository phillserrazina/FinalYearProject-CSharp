using System;
using System.IO;
using System.Collections.Generic;
using Configurate.Tools;
using System.Linq;

namespace Configurate.Managers
{
    class SetupManager
    {

        public Dictionary<string, string> ApplicationInfo = new Dictionary<string, string>();

        public SetupManager()
        {
            SetupIcons();
            SetupCurfs();
            SetupApplicationsFile();
        }

        private void SetupIcons()
        {
            var iconsFiles = Directory.GetFiles(@"../../../Images");

            string iconsDirectory = $"{Defaults.ROAMING}\\Configurate\\Icons";

            foreach (var file in iconsFiles)
            {
                string fileName = file.Split('\\').Last<string>();

                string targetPath = Path.Combine(iconsDirectory, fileName);

                if (!File.Exists(targetPath))
                {
                    if (!Directory.Exists(iconsDirectory))
                    {
                        Directory.CreateDirectory(iconsDirectory);
                    }

                    File.Copy(@"../../../Images/" + fileName, targetPath);
                }
            }
        }

        private void SetupCurfs()
        {
            var curfs = Directory.GetFiles(@"../../../CURFs");

            string curfsDirectory = $"{Defaults.ROAMING}\\Configurate\\CURFs";

            foreach (var file in curfs)
            {
                string fileName = file.Split('\\').Last<string>();

                string targetPath = Path.Combine(curfsDirectory, fileName);

                if (!File.Exists(targetPath))
                {
                    if (!Directory.Exists(curfsDirectory))
                    {
                        Directory.CreateDirectory(curfsDirectory);
                    }

                    File.Copy(@"../../../CURFs/" + fileName, targetPath);
                }
            }
        }

        private void SetupApplicationsFile()
        {
            string setupPath = $"{Defaults.ROAMING}\\Configurate\\Setup\\Applications.txt";

            if (!File.Exists(setupPath))
            {
                if (!Directory.Exists($"{Defaults.ROAMING}\\Configurate\\Setup\\"))
                {
                    Directory.CreateDirectory($"{Defaults.ROAMING}\\Configurate\\Setup\\");
                }

                File.Copy(@"../../../Applications.txt", setupPath);
            }

            string line = "";

            using (var reader = new StreamReader(setupPath))
            {

                while (line != null)
                {
                    line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var valuePair = line.Split('=');
                        string appName = valuePair[0];
                        string appPath = valuePair[1];

                        appPath = appPath.Replace("$DOCUMENTS$", Defaults.DOCUMENTS);
                        appPath = appPath.Replace("$ROAMING$", Defaults.ROAMING);
                        appPath = appPath.Replace("$LOCAL$", Defaults.LOCAL);
                        appPath = appPath.Replace("$LOW$", Defaults.LOW);

                        appPath = appPath.Replace('\\', '/');

                        ApplicationInfo.Add(appName, appPath);
                    }
                }
            }
        }
    }
}
