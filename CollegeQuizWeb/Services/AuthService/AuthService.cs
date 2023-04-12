using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Smtp;
using CollegeQuizWeb.SmtpViewModels;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
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

        var userEntity = await _context.Users.FirstOrDefaultAsync(o => o.Username.Equals(obj.Dto.LoginOrEmail)
                                                                 || o.Email.Equals(obj.Dto.LoginOrEmail));

        if (userEntity != null)
        {
            if (!userEntity.IsAccountActivated)
            {
                controller.ModelState.AddModelError("Password", Lang.UNACTIVATED_ACCOUNT);
            }

            if (controller.ModelState.IsValid)
            {
                if (_passwordHasher.VerifyHashedPassword(userEntity, userEntity.Password, obj.Dto.Password) ==
                    PasswordVerificationResult.Success)
                {
                    if (userEntity.AccountStatus == -1)
                    {
                        controller.ModelState.AddModelError("Password", Lang.ACCOUNT_SUSPENDED);
                    }
                    else
                    {
                        controller.HttpContext.Session.SetString(SessionKey.IS_USER_LOGGED, userEntity.Username);
                        controller.Response.Redirect("/home");
                    }
                }
                else
                {
                    controller.ModelState.AddModelError("Password", Lang.INVALID_PASSWORD);
                }
            }
        }
        else
        {
            controller.ModelState.AddModelError("Password", Lang.INVALID_PASSWORD);
        }
    }

    public async Task Activate(string token, AuthController controller)
    {
        DateTime now = DateTime.Now;
        string responseMessage, viewBagType;

        var tokenEntity = await _context.OtaTokens
            .FirstOrDefaultAsync(t => t.Token.Equals(token) && now < t.ExpiredAt && !t.IsUsed);
        if (tokenEntity == null)
        {
            _logger.LogError("Attempt to proceed request with non existing or invalid token. Token: {}", token);
            viewBagType = "alert-danger";
            responseMessage = Lang.ACCOUNT_ACTIVATION_LINK_EXPIRED;
        }
        else
        {
            tokenEntity.IsUsed = true;
            long chosenUserId = tokenEntity.UserId;
            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.Equals(chosenUserId));
            userEntity!.IsAccountActivated = true;
            _context.Update(tokenEntity);
            _context.Update(userEntity);
            await _context.SaveChangesAsync();
            viewBagType = "alert-success";
            responseMessage = Lang.ACCOUNT_ACTIVATED_SUCCESSFULLY;
        }

        controller.HttpContext.Session.SetString(SessionKey.ACTIVATE_ACCOUNT_REDIRECT, responseMessage);
        controller.HttpContext.Session.SetString(SessionKey.ACTIVATE_ACCOUNT_VIEWBAG_TYPE, viewBagType);
        controller.Response.Redirect("Login");
    }

    public async Task Register(RegisterDtoPayload obj)
    {
        int tokenLife = 48; // in hours

        AuthController controller = obj.ControllerReference;

        UserEntity userEntity = new();
        userEntity.FirstName = obj.Dto.FirstName;
        userEntity.LastName = obj.Dto.LastName;
        userEntity.Username = obj.Dto.Username;
        userEntity.Password = _passwordHasher.HashPassword(userEntity, obj.Dto.Password);
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

        if (!controller.ModelState.IsValid) return;

        string generatedToken;
        bool isExactTheSame;
        do
        {
            generatedToken = Utilities.GenerateOtaToken();
            var token = await _context.OtaTokens.FirstOrDefaultAsync(t => t.Token.Equals(generatedToken));
            isExactTheSame = (token != null);
        } while (isExactTheSame);

        OtaTokenEntity otaToken = new OtaTokenEntity()
        {
            Token = generatedToken,
            ExpiredAt = DateTime.Now.AddHours(tokenLife),
            IsUsed = false,
            UserEntity = userEntity,
        };
        await _context.AddAsync(otaToken);

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
            TemplateName = TemplateName.CONFIRM_ACCOUNT_CREATE,
            ToEmails = new List<string>() { userEntity.Email },
            Subject = string.Format(Lang.EMAIL_ACCOUNT_CRETED_INFROMATION, userEntity.FirstName, userEntity.LastName, userEntity.Username),
            DataModel = emailViewModel
        };
        if (!await _smtpService.SendEmailMessage(options))
        {
            controller.ViewBag.Type = "alert-danger";
            controller.ViewBag.AlertMessage = string.Format(Lang.EMAIL_SENDING_ERROR, userEntity.Email);
        }

        controller.Response.Redirect("/Home");
    }

    public async Task<bool> EmailExistsInDb(string email) =>
        !((await _context.Users.FirstOrDefaultAsync(o => o.Email.Equals(email))) == null);

    public async Task<bool> UsernameExistsInDb(string username) =>
        !((await _context.Users.FirstOrDefaultAsync(o => o.Username.Equals(username))) == null);
}