namespace Configurate.TemplateObjects
{
    class PostTO
    {
        // VARIABLES
        public readonly int ID;
        public readonly string Owner;
        public readonly string Description;
        public readonly string Ratings;

        // CONSTRUCTOR
        public PostTO()
        {
            ID = -1;
            Owner = "Default";
            Description = "Default";
            Ratings = "0,0";
        }
    }
}
