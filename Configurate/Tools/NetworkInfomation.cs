using System.Configuration;

namespace Configurate.Tools
{
    public static class NetworkInfomation
    {
        public static string ConnectionString(string name) => ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }
}
