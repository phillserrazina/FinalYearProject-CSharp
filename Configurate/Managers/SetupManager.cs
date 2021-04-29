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
            try
            {
                // Make tooltips last longer
                ToolTipService.ShowDurationProperty.OverrideMetadata(
                    typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
            } catch { }

            // Create Configurate's AppData folder
            if (!Directory.Exists(Defaults.CONFIGURATE))
            {
                Directory.CreateDirectory(Defaults.CONFIGURATE);
            }

            // Create temporary Server folder. In a release version this server
            // would be an actual server on a web storage
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

            // Retrieve application's from the Setup.ini file
            UpdateApplications();
        }

        // METHODS
        private void SetupParsers()
        {
            try
            {
                // Get all the default Parsers that come included with the project.
                // This path is for the debug version only. The number of "../" might
                // change depending on how the project is built. If you get an error,
                // please inspect it and make sure that the number of "../" is correct.
                // For exporting, this path should be simply @"Parsers"
                string initPath = @"../../../../Parsers";
                var parsers = Directory.GetFiles(initPath);

                // Get Parsers directory
                string parsersDirectory = Defaults.PARSERS;

                // Check if the directory already exists
                if (!Directory.Exists(parsersDirectory))
                {
                    Directory.CreateDirectory(parsersDirectory);
                }

                // Go through all the found parsers
                foreach (var file in parsers)
                {
                    // Get file name from the file path
                    string fileName = file.Split('\\').Last<string>();

                    // Get the target path where the file is to be copied to
                    string targetPath = Path.Combine(parsersDirectory, fileName);

                    // Check if the file already exists
                    if (!File.Exists(targetPath))
                    {
                        // Copy file to the target directory
                        File.Copy(initPath + "/" + fileName, targetPath);
                    }
                }
            }
            catch (Exception e)
            {
                // Handle error exceptions
                MessageBox.Show("Couldn't Setup Parsers: " + e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupAutofills()
        {
            try
            {
                // Get all the default Autofills that come included with the project.
                // This path is for the debug version only. The number of "../" might
                // change depending on how the project is built. If you get an error,
                // please inspect it and make sure that the number of "../" is correct.
                // For exporting, this path should be simply @"Autofills"
                string initPath = @"../../../../Autofills";
                var autofills = Directory.GetDirectories(initPath);

                // Get Autofills directory
                string autofillsDirectory = Defaults.AUTOFILLS;

                // Create Autofills directory if it doesn't exist already
                if (!Directory.Exists(autofillsDirectory))
                {
                    Directory.CreateDirectory(autofillsDirectory);
                }

                // Go through all the found autofill folders
                foreach (var directory in autofills)
                {
                    // Go through all the files inside of the current folder
                    foreach (var file in Directory.GetFiles(directory))
                    {
                        // Get file name from the file path
                        string fileName = file.Split('\\').Last<string>();

                        // Get the target directory where the file is to be copied to
                        string targetPath = Path.Combine(autofillsDirectory, directory.Split('\\').Last<string>());

                        // Check if the directory already exists
                        if (!Directory.Exists(targetPath))
                        {
                            Directory.CreateDirectory(targetPath);
                        }

                        // Get target path for the current file
                        targetPath = Path.Combine(targetPath, fileName);

                        // Check if the file already exists
                        if (!File.Exists(targetPath))
                        {
                            // Copy file to the target directory
                            File.Copy(directory + "\\" + fileName, targetPath);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Handle error exceptions
                MessageBox.Show("Couldn't Setup Autofills: " + e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupIcons()
        {
            try
            {
                // Get all the default icons that come included with the project.
                // This path is for the debug version only. The number of "../" might
                // change depending on how the project is built. If you get an error,
                // please inspect it and make sure that the number of "../" is correct.
                // For exporting, this path should be simply @"Images"
                string initPath = @"../../../../Images";
                var iconsFiles = Directory.GetFiles(initPath);

                // Get Icons directory
                string iconsDirectory = Defaults.ICONS;

                // Check if the directory already exists
                if (!Directory.Exists(iconsDirectory))
                {
                    Directory.CreateDirectory(iconsDirectory);
                }

                // Go through all the found icons
                foreach (var file in iconsFiles)
                {
                    // Get file name from the file path
                    string fileName = file.Split('\\').Last<string>();

                    // Get the target path where the file is to be copied to
                    string targetPath = Path.Combine(iconsDirectory, fileName);

                    // Check if the file already exists
                    if (!File.Exists(targetPath))
                    {
                        // Copy file to the target directory
                        File.Copy(initPath + "/" + fileName, targetPath);
                    }
                }
            }
            catch (Exception e)
            {
                // Handle error exceptions
                MessageBox.Show("Couldn't Setup Icons: " + e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupCurfs()
        {
            try
            {
                // Get all the default CURFs that come included with the project.
                // This path is for the debug version only. The number of "../" might
                // change depending on how the project is built. If you get an error,
                // please inspect it and make sure that the number of "../" is correct.
                // For exporting, this path should be simply @"CURFs"
                string initPath = @"../../../../CURFs";
                var curfs = Directory.GetFiles(initPath);

                // Get Icons directory
                string curfsDirectory = Defaults.CURFS;

                // Check if the directory already exists
                if (!Directory.Exists(curfsDirectory))
                {
                    Directory.CreateDirectory(curfsDirectory);
                }

                // Go through all the found CURFs
                foreach (var file in curfs)
                {
                    // Get file name from the file path
                    string fileName = file.Split('\\').Last<string>();

                    // Get the target path where the file is to be copied to
                    string targetPath = Path.Combine(curfsDirectory, fileName);

                    // Check if the file already exists
                    if (!File.Exists(targetPath))
                    {
                        // Copy file to the target directory
                        File.Copy(initPath + "/" + fileName, targetPath);
                    }
                }
            }
            catch (Exception e)
            {
                // Handle error exceptions
                MessageBox.Show("Couldn't Setup CURFs: " + e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupApplicationsFile()
        {
            try
            {
                // Get the target path where the file is to be copied to
                string newSetupPath = $"{Defaults.CONFIGURATE}\\Setup.ini";

                // Check if the file already exists
                if (!File.Exists(newSetupPath))
                {
                    // Copy file to the target directory
                    // This path is for the debug version only. The number of "../" might
                    // change depending on how the project is built. If you get an error,
                    // please inspect it and make sure that the number of "../" is correct.
                    // For exporting, this path should be simply @"Setup.ini"
                    File.Copy(@"../../../../Setup.ini", newSetupPath);
                }
            }
            catch (Exception e)
            {
                // Handle error exceptions
                MessageBox.Show("Couldn't Setup Setup.ini: " + e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateApplications()
        {
            try
            {
                // Parse the Setup.ini file
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile($"{Defaults.CONFIGURATE}\\Setup.ini");
                var allSections = data.Sections;

                // Reset the ApplicationInfo list
                ApplicationInfo.Clear();

                // Go through all the sections (aka applications)
                foreach (var section in allSections)
                {
                    // Skip all applications that have Active = false
                    if (section.Keys["Active"] == "false") continue;

                    // Add application to the ApplicationInfo list
                    var appSetupInfo = new ApplicationSetupInfoTO(section);
                    ApplicationInfo.Add(appSetupInfo);
                }
            }
            catch (Exception e)
            {
                // Handle error exceptions
                MessageBox.Show("Couldn't Setup Application's List: " + e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
