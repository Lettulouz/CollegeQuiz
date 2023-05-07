using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CollegeQuizWeb.Config;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Liquid;
using FluentEmail.Smtp;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CollegeQuizWeb.Smtp;

public class SmtpService : ISmtpService
{
    private readonly ILogger<SmtpService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    private const string TEMPLATE_PATH = @"EmailTemplates/{0}.liquid";
    private const string TEMPLATE_SUFFIX = "Template";
    private const string TEMPLATE_DIR = "EmailTemplates";
    
    public SmtpService(IWebHostEnvironment hostEnvironment, ILogger<SmtpService> logger, ApplicationDbContext context)
    {
        _hostEnvironment = hostEnvironment;
        _logger = logger;
        _context = context;
    }
    
    public async Task<bool> SendEmailMessage<T>(UserEmailOptions<T> userEmailOptions) where T: AbstractSmtpViewModel
    {
        try
        {
            string templateFilesPath = _hostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + TEMPLATE_DIR;
            PhysicalFileProvider fileProvider = new PhysicalFileProvider(templateFilesPath);
            LiquidRendererOptions options = new LiquidRendererOptions()
            {
                FileProvider = fileProvider,
            };
            SmtpClient smtpClient = new SmtpClient()
            {
                Host = ConfigLoader.SmtpHost,
                Port = ConfigLoader.SmtpPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(ConfigLoader.SmtpSender, ConfigLoader.SmtpPassword)
            };
            Email.DefaultSender = new SmtpSender(smtpClient);
            Email.DefaultRenderer = new LiquidRenderer(Options.Create(options));

            userEmailOptions.DataModel.CurrentYear = DateTime.Now.Year.ToString();
            userEmailOptions.DataModel.CurrentDate = DateTime.Now.ToString(CultureInfo.DefaultThreadCurrentCulture);
                    
            string templatePath = string.Format(TEMPLATE_PATH, userEmailOptions.TemplateName + TEMPLATE_SUFFIX);
            string templateRawContent = File.ReadAllText(templatePath);

            List<Address> addresses = new List<Address>();
            foreach (string user in userEmailOptions.ToEmails) addresses.Add(new Address(user));
            
            IFluentEmail fluentEmail = Email
                .From(ConfigLoader.SmtpSender, ConfigLoader.SmtpName)
                .ReplyTo(ConfigLoader.SmtpLoopback, ConfigLoader.SmtpName)
                .To(addresses)
                .Subject($"Quizazu | {userEmailOptions.Subject}")
                .UsingTemplate(templateRawContent, userEmailOptions.DataModel);
        
            SendResponse sendResponse = await fluentEmail.SendAsync();
            return sendResponse.Successful;
        }
        catch (Exception ex)
        {
            SubscriptionTypesEntity test = new();
            test.Name = ex.Message;
            test.Price = 0;
            test.SiteId = 999;
            test.BeforeDiscountPrice = 0;

            _context.SubsciptionTypes.Add(test);
            await _context.SaveChangesAsync();
            _logger.LogError("Unable to send email message. Cause: {}", ex.Message);
            return false;
        }
    }
}