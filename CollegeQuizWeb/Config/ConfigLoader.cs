using DotNetEnv;

namespace CollegeQuizWeb.Config;

public class ConfigLoader
{
    public static string DbConnectionString { get; set; }
    public static string DbVersion { get; set; }
    
    public static void InsertEnvProperties(IConfiguration configuration)
    {
        Env.Load();
        configuration.GetSection("Configuration").Bind(new ConfigLoader());
        
        // env properties
        DbConnectionString = Env.GetString("MY_SEQUEL_CONNECTION");
    }
}