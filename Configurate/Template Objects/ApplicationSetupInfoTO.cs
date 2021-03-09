namespace Configurate.TemplateObjects
{
    class ApplicationSetupInfoTO
    {
        public readonly string Name;
        public readonly string Path;
        public readonly string ParserFile;
        public readonly string SaverFile;

        public ApplicationSetupInfoTO(string Name, string Path, string ParserFile, string SaverFile)
        {
            this.Name = Name;
            this.Path = Path;
            this.ParserFile = ParserFile;
            this.SaverFile = SaverFile;
        }
    }
}
