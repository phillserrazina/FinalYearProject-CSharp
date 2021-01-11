using System;
using System.IO;
using System.Collections.Generic;
using Configurate.TemplateObjects;

namespace Configurate.Managers
{
    class ApplicationsManager
    {
        // VARIABLES
        public static List<ApplicationInfoTO> ApplicationsList { get; private set; }
        public static ApplicationInfoTO CurrentApplication;
        public static List<SettingsTO> SettingsList;

        // EXECUTION FUNCTIONS
        public ApplicationsManager(List<ApplicationInfoTO> appList)
        {
            ApplicationsList = appList;
        }

        public static void CreateDefault()
        {
            ApplicationsList = new List<ApplicationInfoTO>
            {
                new ApplicationInfoTO("Skyrim", $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/My Games/Skyrim/Skyrim.ini"),
                new ApplicationInfoTO("Skyrim Special Edition", $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/My Games/Skyrim Special Edition/Skyrim.ini"),
                new ApplicationInfoTO("Witcher 3", "D:/QT Projects/ConfigurateProject/TestSaveFiles/witcherConfigTest.txt"),
                new ApplicationInfoTO("Sun Rings", ""),
                new ApplicationInfoTO("Darkest Dungeon", $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/Darkest/persist.options.json")
            };
        }

        // METHODS
        public static void Add(ApplicationInfoTO appInfo)
        {
            foreach (var app in ApplicationsList)
            {
                if (app.Path.Equals(appInfo.Path))
                {
                    // TODO: Print Error Message
                    return;
                }

                ApplicationsList.Add(appInfo);
            }
        }

        public static void Remove(ApplicationInfoTO appInfo)
        {
            if (ApplicationsList.Contains(appInfo)) ApplicationsList.Remove(appInfo);
        }

        public static ApplicationInfoTO GetApplication(string appName)
        {
            foreach (var app in ApplicationsList)
            {
                if (app.Name.Equals(appName))
                    return app;
            }

            // TODO: Print Error Message
            return null;
        }
    }
}
