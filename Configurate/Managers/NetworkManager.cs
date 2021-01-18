using Dapper;
using System.Data;
using System.Linq;
using Configurate.Tools;
using System.Data.SqlClient;
using Configurate.TemplateObjects;

namespace Configurate.Managers
{
    class NetworkManager
    {
        public static UserTO CurrentUser { get; private set; }

        public static bool LoggedIn { get { return CurrentUser != null; } }

        public static (UserTO, string) GetUser(string username, string password)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(NetworkInfomation.ConnectionString("ConfigurateDB")))
                {
                    UserTO result = null;

                    try
                    {
                        result = connection.Query<UserTO>($"SELECT * FROM Users WHERE username = '{username}'").ToList()[0];
                    }
                    catch
                    {
                        return (null, $"The username { username } does not exist. Please try again.");
                    }

                    if (result.Userpsw != password) return (null, "Wrong password. Please try again.");

                    return (result, "All good.");
                }
            }
            catch
            {
                return (null, "Couldn't connect to database. Please try again."); 
            }
        }

        public static void LogIn(UserTO user) => CurrentUser = user;
    }
}
