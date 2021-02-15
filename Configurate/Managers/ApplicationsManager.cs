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
        }

        // This is technically not a constructor, but it is an initialization method in case you ever want
        // to pass the application list manually.
        // This was made because static constructors do NOT accept parameters.
        public static void Initialize(List<ApplicationInfoTO> appList) {
            ApplicationsList = appList;
        }

        public static void Initialize(Dictionary<string, string> appDic)
        {
            ApplicationsList = new List<ApplicationInfoTO>();

            foreach (var key in appDic.Keys)
            {
                ApplicationsList.Add(new ApplicationInfoTO(key, appDic[key]));    
            }
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
