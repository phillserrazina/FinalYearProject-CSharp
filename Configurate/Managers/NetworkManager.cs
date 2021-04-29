using Dapper;

using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

using Configurate.Tools;
using Configurate.TemplateObjects;

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
                // Connect to the Configurate Database
                using (IDbConnection connection = new SqlConnection(NetworkInfomation.ConnectionString("ConfigurateDB")))
                {
                    UserTO result = null;

                    try
                    {
                        // Run SQL query
                        result = connection.Query<UserTO>($"SELECT * FROM Users WHERE username = '{username}'").ToList()[0];
                    }
                    catch
                    {
                        // Return an error if SQL query fails
                        return (null, $"The username { username } does not exist. Please try again.");
                    }

                    // Check if password is correct, return an error if it is not
                    if (result.Userpsw != password) return (null, "Wrong password. Please try again.");

                    // Return successfull result
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
                // Connect to the Configurate Database
                using (IDbConnection connection = new SqlConnection(NetworkInfomation.ConnectionString("ConfigurateDB")))
                {
                    List<PostTO> result = null;

                    try
                    {
                        // Run SQL query
                        result = new List<PostTO>(connection.Query<PostTO>($"SELECT * FROM Posts WHERE game = '{game}'").ToList());
                    }
                    catch
                    {
                        // Return an error if SQL query fails
                        return (null, $"There are no posts for { game }. Please try again.");
                    }

                    // Return successfull result
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
                // Connect to the Configurate Database
                using (IDbConnection connection = new SqlConnection(NetworkInfomation.ConnectionString("ConfigurateDB")))
                {
                    try
                    {
                        // Run async SQL query
                        string query = $"INSERT INTO Posts (owner, description, ratings, game) OUTPUT INSERTED.id VALUES ('{owner}', '{description}', '0,0', '{game}')";
                        var result = await connection.QueryAsync<PostTO>(query);
                        return (result.FirstOrDefault(), "Posted succeessfully.");
                    }
                    catch (Exception e)
                    {
                        // Return an error if SQL query fails
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
