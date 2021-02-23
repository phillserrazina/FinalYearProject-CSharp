﻿using System;
using System.IO;
using System.Collections.Generic;
using Configurate.Tools;
using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace Configurate.Managers
{
    class SetupManager
    {
        // VARIABLES
        public Dictionary<string, string> ApplicationInfo = new Dictionary<string, string>();

        // CONSTRUCTOR
        public SetupManager()
        {
            // Make tooltips last longer
            ToolTipService.ShowDurationProperty.OverrideMetadata(
                typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            // Set up AppData files
            SetupIcons();
            SetupCurfs();
            SetupApplicationsFile();
        }

        // METHODS
        private void SetupIcons()
        {
            var iconsFiles = Directory.GetFiles(@"../../../Images");

            string iconsDirectory = Defaults.ICONS;

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

            string curfsDirectory = Defaults.CURFS;

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
            string setupPath = $"{Defaults.SETUP}\\Applications.txt";

            if (!File.Exists(setupPath))
            {
                if (!Directory.Exists(Defaults.SETUP))
                {
                    Directory.CreateDirectory(Defaults.SETUP);
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
