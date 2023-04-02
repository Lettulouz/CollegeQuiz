using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Smtp;
using CollegeQuizWeb.SmtpViewModels;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly ISmtpService _smtpService;

    public AuthService(ILogger<AuthService> logger, ApplicationDbContext context, ISmtpService smtpService,
        IPasswordHasher<UserEntity> passwordHasher)
    {
        _logger = logger;
        _context = context;
        _smtpService = smtpService;
        _passwordHasher = passwordHasher;
    }

    public async Task Login(LoginDtoPayload obj)
    {
        AuthController controller = obj.ControllerReference;
        
        var arek = await _context.Users.FirstOrDefaultAsync(o => o.Username.Equals(obj.Dto.LoginOrEmail)
                                                                 || o.Email.Equals(obj.Dto.LoginOrEmail));

        if (arek!=null)
        {
            if (arek.IsAccountActivated == 0)
            {
                controller.ModelState.AddModelError("Password", Lang.UNACTIVATED_ACCOUNT);
            }

            if (_passwordHasher.VerifyHashedPassword(arek, arek.Password, obj.Dto.Password) ==
                PasswordVerificationResult.Success)
            {
                controller.Response.Redirect("/home");
            }
            else
            {
                controller.ModelState.AddModelError("Password", Lang.INVALID_PASSWORD);
            }
        }
        else
        {
            controller.ModelState.AddModelError("Password", Lang.INVALID_PASSWORD);
        }

            
    }

    public async Task Register(RegisterDtoPayload obj)
    {
        int tokenLife = 2880;
        
        AuthController controller = obj.ControllerReference;

        UserEntity userEntity = new();
        userEntity.FirstName = obj.Dto.FirstName;
        userEntity.LastName = obj.Dto.LastName;
        userEntity.Username = obj.Dto.Username;
        userEntity.Password = obj.Dto.Password == null ? "" : _passwordHasher.HashPassword(userEntity, obj.Dto.Password);
        userEntity.Email = obj.Dto.Email;
        userEntity.TeamID = obj.Dto.TeamID;
        userEntity.RulesAccept = obj.Dto.RulesAccept;


        if (await EmailExistsInDb(obj.Dto.Email))
        {
            controller.ModelState.AddModelError("Email", Lang.EMAIL_ALREADY_EXIST);
        }
        if (await UsernameExistsInDb(obj.Dto.Username))
        {
            controller.ModelState.AddModelError("Username", Lang.USERNAME_ALREADY_EXIST);
        }
        if (obj.Dto.RulesAccept.Equals(false))
        {
            controller.ModelState.AddModelError("RulesAccept", Lang.RULES_ACCEPT);
        }

        if (controller.ModelState.IsValid)
        {
            string generatedToken;
            bool isExactTheSame = false;
            do
            {
                generatedToken = Utilities.GenerateOtaToken();
                var token = await _context.OtaTokens.FirstOrDefaultAsync(t => t.Token.Equals(generatedToken));
                if (token != null) isExactTheSame = true;
            } while (isExactTheSame);
            
            OtaTokenEntity otaToken = new OtaTokenEntity()
            {
                Token = generatedToken,
                ExpiredAt = DateTime.Now.AddMinutes(tokenLife),
                IsUsed = false,
                UserEntity = userEntity,
            };
            await _context.AddAsync(otaToken);
            await _context.SaveChangesAsync();
            
            var uriBuilder = new UriBuilder(controller.Request.Scheme, controller.Request.Host.Host,
                controller.Request.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort) uriBuilder.Port = -1;
            
            await _context.AddAsync(userEntity);
            await _context.SaveChangesAsync();
            ConfirmAccountSmtpViewModel emailViewModel = new()
            {
                FullName = $"{userEntity.FirstName} {userEntity.LastName}",
                TokenValidTime = tokenLife,
                ConfirmAccountLink = $"{uriBuilder.Uri.AbsoluteUri}Auth/ConfirmAccount?token={generatedToken}",
            };
            UserEmailOptions<ConfirmAccountSmtpViewModel> options = new()
            {
                TemplateName = TemplateName.CHANGE_PASSWORD,
                ToEmails = new List<string>() { userEntity.Email },
                Subject = $"Tworzenie konta dla {userEntity.FirstName} {userEntity.LastName} ({userEntity.Username})",
                DataModel = emailViewModel
            };
            if (!await _smtpService.SendEmailMessage(options))
            {
                controller.ViewBag.Type = "alert-danger";
                controller.ViewBag.AlertMessage = 
                    $"Nieudane wysłanie wiadomości email na adres {userEntity.Email}. Spróbuj ponownie później.";
            }
            controller.Response.Redirect("/Home");
        }
    }

    public async Task<bool> EmailExistsInDb(string email)
    {
        if (await _context.Users.FirstOrDefaultAsync(o => o.Email.Equals(email)) == null)
            return false;
        return true;
    }
    
    public async Task<bool> UsernameExistsInDb(string username)
    {
        if (await _context.Users.FirstOrDefaultAsync(o => o.Username.Equals(username)) == null)
            return false;
        return true;
    }
}
