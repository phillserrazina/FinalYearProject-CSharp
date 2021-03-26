using System;
using System.IO;
using System.Collections.Generic;
using Configurate.Tools;
using Configurate.TemplateObjects;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using IniParser;
using IniParser.Model;

namespace Configurate.Managers
{
    class SetupManager
    {
        // VARIABLES
        public List<ApplicationSetupInfoTO> ApplicationInfo = new List<ApplicationSetupInfoTO>();

        // CONSTRUCTOR
        public SetupManager()
        {
            // Make tooltips last longer
            ToolTipService.ShowDurationProperty.OverrideMetadata(
                typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            if (!Directory.Exists(Defaults.CONFIGURATE))
            {
                Directory.CreateDirectory(Defaults.CONFIGURATE);
            }

            if (!Directory.Exists($"{Defaults.CONFIGURATE}\\Server"))
            {
                Directory.CreateDirectory($"{Defaults.CONFIGURATE}\\Server");
            }

            // Set up AppData files
            SetupIcons();
            SetupCurfs();
            SetupParsers();
            SetupAutofills();
            SetupApplicationsFile();
        }

        // METHODS
        private void SetupParsers()
        {
            var parsers = Directory.GetFiles(@"../../../Parsers");

            string parsersDirectory = Defaults.PARSERS;

            foreach (var file in parsers)
            {
                string fileName = file.Split('\\').Last<string>();

                string targetPath = Path.Combine(parsersDirectory, fileName);

                if (!File.Exists(targetPath))
                {
                    if (!Directory.Exists(parsersDirectory))
                    {
                        Directory.CreateDirectory(parsersDirectory);
                    }

                    File.Copy(@"../../../Parsers/" + fileName, targetPath);
                }
            }
        }

        private void SetupAutofills()
        {
            var autofills = Directory.GetDirectories(@"../../../Autofills");

            string autofillsDirectory = Defaults.AUTOFILLS;

            if (!Directory.Exists(autofillsDirectory))
            {
                Directory.CreateDirectory(autofillsDirectory);
            }

            foreach (var directory in autofills)
            {
                foreach (var file in Directory.GetFiles(directory))
                {
                    string fileName = file.Split('\\').Last<string>();

                    string targetPath = Path.Combine(autofillsDirectory, directory.Split('\\').Last<string>());

                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }

                    targetPath = Path.Combine(targetPath, fileName);

                    if (!File.Exists(targetPath))
                    {
                        File.Copy(directory + "\\" + fileName, targetPath);
                    }
                }
            }
        }

        private void SetupIcons()
        {
            var iconsFiles = Directory.GetFiles(@"../../../Images");

            string iconsDirectory = Defaults.ICONS;

            if (!Directory.Exists(iconsDirectory))
            {
                Directory.CreateDirectory(iconsDirectory);
            }

            foreach (var file in iconsFiles)
            {
                string fileName = file.Split('\\').Last<string>();

                string targetPath = Path.Combine(iconsDirectory, fileName);

                if (!File.Exists(targetPath))
                {
                    File.Copy(@"../../../Images/" + fileName, targetPath);
                }
            }
        }

        private void SetupCurfs()
        {
            var curfs = Directory.GetFiles(@"../../../CURFs");

            string curfsDirectory = Defaults.CURFS;

            if (!Directory.Exists(curfsDirectory))
            {
                Directory.CreateDirectory(curfsDirectory);
            }

            foreach (var file in curfs)
            {
                string fileName = file.Split('\\').Last<string>();

                string targetPath = Path.Combine(curfsDirectory, fileName);

                if (!File.Exists(targetPath))
                {
                    File.Copy(@"../../../CURFs/" + fileName, targetPath);
                }
            }
        }

        private void SetupApplicationsFile()
        {
            string newSetupPath = $"{Defaults.CONFIGURATE}\\Setup.ini";

            if (!File.Exists(newSetupPath))
            {
                if (!Directory.Exists(Defaults.SETUP))
                {
                    Directory.CreateDirectory(Defaults.SETUP);
                }

                File.Copy(@"../../../Setup.ini", newSetupPath);
            }

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(newSetupPath);
            var allSections = data.Sections;

            foreach (var section in allSections)
            {
                if (section.Keys["Active"] == "false") continue;

                var appSetupInfo = new ApplicationSetupInfoTO(section);
                ApplicationInfo.Add(appSetupInfo);
            }
        }
    }
}
