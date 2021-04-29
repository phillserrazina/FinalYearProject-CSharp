namespace Configurate.TemplateObjects
{
    class UserTO
    {
        // VARIABLES
        public readonly string Username;
        public readonly string Userpsw;
        public readonly string Posts;
        public readonly string RatedPosts;

        // CONSTRUCTOR
        public UserTO()
        {
            Username = "Default";
            Userpsw = "Default";
            Posts = "-1";
            RatedPosts = "-1";
        }
    }
}
