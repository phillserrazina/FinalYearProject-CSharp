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
        private static UserTO currentUser = null;

        public bool LoggedIn { get { return currentUser != null; } }

        public static (UserTO, string) LogIn(string username, string password)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(NetworkInfomation.ConnectionString("ConfigurateDB")))
                {
                    UserTO result = connection.Query<UserTO>($"SELECT * FROM Users WHERE username = '{username}'").ToList()[0];

                    if (result == null) return (null, $"The username { username } does not exist. Please try again.");
                    if (result.Userpsw != password) return (null, "Wrong password. Please try again.");

                    currentUser = result;
                    return (result, "All good.");
                }
            }
            catch
            {
                return (null, "Couldn't connect to database. Please try again."); 
            }
        }
    }
}
