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

        // This is technically not a constructor, but it is an initialization method in order 
        // to inject the setup application list manually.
        public static void Initialize(List<ApplicationSetupInfoTO> appList)
        {
            ApplicationsList = new List<ApplicationInfoTO>();

            foreach (var app in appList)
            {
                ApplicationsList.Add(new ApplicationInfoTO(app));    
            }
        }
    }
}
