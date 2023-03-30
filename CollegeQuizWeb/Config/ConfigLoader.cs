using DotNetEnv;
using Microsoft.Extensions.Configuration;

namespace CollegeQuizWeb.Config;

public class ConfigLoader
{
    public static string DbConnectionString { get; set; }
    public static string DbVersion { get; set; }
    public static string SmtpSender { get; set; }
    public static string SmtpLoopback { get; set; }
    public static string SmtpName { get; set; }
    public static string SmtpHost { get; set; }
    public static int SmtpPort { get; set; }
    public static string SmtpPassword { get; set; }

    public static void InsertEnvProperties(IConfiguration configuration)
    {
        Env.Load();
        configuration.GetSection("Configuration").Bind(new ConfigLoader());
        
        // env properties
        DbConnectionString = Env.GetString("MY_SEQUEL_CONNECTION");
        SmtpHost = Env.GetString("SMTP_HOST");
        SmtpPassword = Env.GetString("SMTP_PASSWORD");
    }
}