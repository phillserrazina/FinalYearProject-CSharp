using Configurate.Tools;
using IniParser.Model;

namespace Configurate.TemplateObjects
{
    class ApplicationSetupInfoTO
    {
        // VARIABLES
        public readonly string Name;
        public readonly string Path;
        public readonly string ParserFile;
        public readonly string SaverFile;

        public readonly string Description;
        public readonly string Developer;
        public readonly string Publisher;
        public readonly string ReleaseDate;

        // CONSTRUCTOR
        public ApplicationSetupInfoTO(SectionData data)
        {
            Name = data.SectionName;

            string appPath = data.Keys["Path"];

            appPath = appPath.Replace("$DOCUMENTS$", Defaults.DOCUMENTS);
            appPath = appPath.Replace("$ROAMING$", Defaults.ROAMING);
            appPath = appPath.Replace("$LOCAL$", Defaults.LOCAL);
            appPath = appPath.Replace("$LOW$", Defaults.LOW);

            appPath = appPath.Replace('\\', '/');

            Path = appPath;

            ParserFile = data.Keys["Parser"];
            SaverFile = data.Keys["Saver"];
            Description = data.Keys["Description"];
            Developer = data.Keys["Developer"];
            Publisher = data.Keys["Publisher"];
            ReleaseDate = data.Keys["Release Date"];
        }
    }
}
