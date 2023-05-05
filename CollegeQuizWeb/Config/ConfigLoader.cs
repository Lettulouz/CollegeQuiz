using System;
using System.Collections.Generic;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PayU.Client;
using PayU.Client.Configurations;
using Swashbuckle.AspNetCore.SwaggerGen;

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
    public static string JwtSecret { get; set; }
    public static int JwtExpiredDays { get; set; }
    public static string JwtIssuer { get; set; }

    public static string PayuClientId { get; set; }
    public static PayUClientSettings PayUClientSettings { get; set; }
    
    public static void InsertEnvProperties(IConfiguration configuration)
    {
        Env.Load();
        configuration.GetSection("Configuration").Bind(new ConfigLoader());
        
        // env properties
        DbConnectionString = Env.GetString("MY_SEQUEL_CONNECTION");
        JwtSecret = Env.GetString("JWT_SECRET");
        JwtIssuer = Env.GetString("JWT_ISSUER");
        
        SmtpSender = "info@quizazu.pl";
        SmtpHost = Env.GetString("SMTP_HOST");
        SmtpPassword = Env.GetString("SMTP_PASSWORD");
        
        PayuClientId = Env.GetString("CLIENT_ID");
        PayUClientSettings = new PayUClientSettings(
            PayUContainer.PayUApiUrl.Sandbox, // Url You could use string example from configuration or use const
            "v2_1", // api version
            PayuClientId , // clientId from shop configuration
            Env.GetString("CLIENT_SECRET") // clientId from shop configuration
        );
    }

    public static Action<SwaggerGenOptions> GetSwaggerConfiguration()
    {
        return c =>
        {
            c.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        };
    }
}