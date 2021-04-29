using System.Configuration;

namespace Configurate.Tools
{
    public static class NetworkInfomation
    {
        // Safely get a connection string (used by the NetworkManager)
        public static string ConnectionString(string name) => ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }
}
