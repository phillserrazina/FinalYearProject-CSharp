using Dapper;
using System.Data;
using System.Linq;
using Configurate.Tools;
using System.Data.SqlClient;
using System.Collections.Generic;
using Configurate.TemplateObjects;
using System.Threading.Tasks;

namespace Configurate.Managers
{
    class NetworkManager
    {
        // VARIABLES
        public static UserTO CurrentUser { get; private set; }

        public static bool LoggedIn { get { return CurrentUser != null; } }

        // METHODS
        public static void LogIn(UserTO user) => CurrentUser = user;

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

        public static (PostTO, string) GetPost(int id)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(NetworkInfomation.ConnectionString("ConfigurateDB")))
                {
                    PostTO result = null;

                    try
                    {
                        result = connection.Query<PostTO>($"SELECT * FROM Posts WHERE id = '{id}'").ToList()[0];
                    }
                    catch
                    {
                        return (null, $"Post with ID = { id } does not exist. Please try again.");
                    }

                    return (result, "All good.");
                }
            }
            catch
            {
                return (null, "Couldn't connect to database. Please try again.");
            }
        }

        public static (List<PostTO>, string) GetPostsOfGame(string game)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(NetworkInfomation.ConnectionString("ConfigurateDB")))
                {
                    List<PostTO> result = null;

                    try
                    {
                        result = new List<PostTO>(connection.Query<PostTO>($"SELECT * FROM Posts WHERE game = '{game}'").ToList());
                    }
                    catch
                    {
                        return (null, $"There are no posts for { game }. Please try again.");
                    }

                    return (result, "All good.");
                }
            }
            catch
            {
                return (null, "Couldn't connect to database. Please try again.");
            }
        }

        public async static Task<(PostTO, string)> AddPost(string owner, string description, string game)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(NetworkInfomation.ConnectionString("ConfigurateDB")))
                {
                    try
                    {
                        //var result = connection.Query<PostTO>($"INSERT INTO Posts (owner, description, ratings, game) VALUES ('{owner}', '{description}', '0,0', '{game}')").ToList()[0];
                        var result = await connection.QueryAsync<PostTO>($"INSERT INTO Posts (owner, description, ratings, game) OUTPUT INSERTED.id VALUES ('{owner}', '{description}', '0,0', '{game}')");
                        return (result.FirstOrDefault(), "Posted succeessfully.");
                    }
                    catch (System.Exception e)
                    {
                        return (null, $"Couldn't post ({e.Message}). Try again.");
                    }
                }
            }
            catch
            {
                return (null, "Couldn't connect to database. Please try again.");
            }
        }
    }
}
