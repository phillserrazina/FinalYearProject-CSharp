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

        public delegate void OnDirtyDelegate(bool val);
        public static OnDirtyDelegate OnDirty;

        public static bool IsDirty { get; private set; }

        // CONSTRUCTORS
        static ApplicationsManager()
        {
            OnDirty += (bool val) => { IsDirty = val; };

            ApplicationsList = new List<ApplicationInfoTO>
            {
                new ApplicationInfoTO("Skyrim", $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/My Games/Skyrim/Skyrim.ini"),
                new ApplicationInfoTO("Skyrim Special Edition", $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/My Games/Skyrim Special Edition/Skyrim.ini"),
                new ApplicationInfoTO("Witcher 3", "D:/QT Projects/ConfigurateProject/TestSaveFiles/witcherConfigTest.txt"),
                new ApplicationInfoTO("Sun Rings", ""),
                new ApplicationInfoTO("Darkest Dungeon", $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/Darkest/persist.options.json")
            };
        }

        // This is technically not a constructor, but it is an initialization method in case you ever want
        // to pass the application list manually.
        // This was made because static constructors do NOT accept parameters.
        public static void Init(List<ApplicationInfoTO> appList) {
            ApplicationsList = appList;
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
