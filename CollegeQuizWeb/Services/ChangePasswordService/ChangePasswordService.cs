using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.ChangePassword;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Smtp;
using CollegeQuizWeb.SmtpViewModels;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.Services.ChangePasswordService;

public class ChangePasswordService : IChangePasswordService
{
    private static readonly int TOKEN_LIFE = 10;
    
    private readonly ILogger<ChangePasswordService> _logger;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly ApplicationDbContext _context;
    private readonly ISmtpService _smtpService;

    public ChangePasswordService(
        ApplicationDbContext context, ISmtpService smtpService, ILogger<ChangePasswordService> logger,
        IPasswordHasher<UserEntity> passwordHasher)
    {
        _context = context;
        _smtpService = smtpService;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payloadDto"></param>
    public async Task AttemptChangePassword(AttemptChangePasswordPayloadDto payloadDto)
    {
        AuthController controller = payloadDto.ControllerReference;
        if (!controller.ModelState.IsValid) return;

        string loginOrEmail = payloadDto.Dto.LoginOrEmail;
        var userEntity = await _context.Users
            .FirstOrDefaultAsync(u => u.Username.Equals(loginOrEmail) || u.Email.Equals(loginOrEmail));
        if (userEntity == null)
        {
            controller.ViewBag.Type = "alert-danger";
            controller.ViewBag.AlertMessage = "Nie znaleziono użytkownika na podstawie przekazanych danych.";
            _logger.LogError("User with passed data: {} not exist", payloadDto.Dto.LoginOrEmail);
            return;
        }
        bool isExactTheSame = false;
        string generatedToken;
        do
        {
            generatedToken = Utilities.GenerateOtaToken();
            var token = await _context.OtaTokens.FirstOrDefaultAsync(t => t.Token.Equals(generatedToken));
            if (token != null) isExactTheSame = true;
        } while (isExactTheSame);
        
        OtaTokenEntity otaToken = new OtaTokenEntity()
        {
            Token = generatedToken,
            ExpiredAt = DateTime.Now.AddMinutes(TOKEN_LIFE),
            IsUsed = false,
            UserEntity = userEntity,
        };

        var uriBuilder = new UriBuilder(controller.Request.Scheme, controller.Request.Host.Host,
            controller.Request.Host.Port ?? -1);
        if (uriBuilder.Uri.IsDefaultPort) uriBuilder.Port = -1;
        
        ChangePasswordSmtpViewModel emailViewModel = new ChangePasswordSmtpViewModel()
        {
            FullName = $"{userEntity.FirstName} {userEntity.LastName}",
            TokenValidTime = TOKEN_LIFE,
            ResetPasswordLink = $"{uriBuilder.Uri.AbsoluteUri}Auth/ChangePassword?token={generatedToken}",
        };
        UserEmailOptions<ChangePasswordSmtpViewModel> options = new UserEmailOptions<ChangePasswordSmtpViewModel>()
        {
            TemplateName = TemplateName.CHANGE_PASSWORD,
            ToEmails = new List<string>() { userEntity.Email },
            Subject = $"Reset hasła dla {userEntity.FirstName} {userEntity.LastName} ({userEntity.Username})",
            DataModel = emailViewModel
        };
        if (!await _smtpService.SendEmailMessage(options))
        {
            controller.ViewBag.Type = "alert-danger";
            controller.ViewBag.AlertMessage = 
                $"Nieudane wysłanie wiadomości email na adres {userEntity.Email}. Spróbuj ponownie później.";
        }
        
        await _context.AddAsync(otaToken);
        await _context.SaveChangesAsync();
        
        controller.ViewBag.Type = "alert-success";
        controller.ViewBag.AlertMessage = 
            $"Na adres email <strong>{userEntity.Email}</strong> została wysłana wiadomość z linkiem umożliwiającym " +
            $"zmianę hasła.";
        
        _logger.LogInformation("Send request to change password for account: {}", userEntity.Email);
        controller.ModelState.SetModelValue("LoginOrEmail", new ValueProviderResult(string.Empty, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="controller"></param>
    public async Task CheckBeforeChangePassword(string token, AuthController controller)
    {
        DateTime now = DateTime.Now;
        
        var tokenEntity = await _context.OtaTokens
            .FirstOrDefaultAsync(t => t.Token.Equals(token) && now < t.ExpiredAt && !t.IsUsed);
        if (tokenEntity == null)
        {
            _logger.LogError("Attempt to proceed request with non existing or invalid token. Token: {}", token);
            controller.ViewBag.DisableChangePasswordView = true;
        }
        controller.ViewBag.Token = token;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="payloadDto"></param>
    public async Task ChangePassword(string token, ChangePasswordPayloadDto payloadDto)
    {
        AuthController controller = payloadDto.ControllerReference;
        controller.ViewBag.Token = token;
        if (!controller.ModelState.IsValid) return;

        if (!payloadDto.Dto.NewPassword.Equals(payloadDto.Dto.RepeatNewPassword))
        {
            controller.ViewBag.Type = "alert-danger";
            controller.ViewBag.AlertMessage = $"Wartości w polach nowego hasła i potwórzonego hasła nie są takie same.";
            return;
        }
        
        DateTime now = DateTime.Now;
        var tokenEntity = await _context.OtaTokens
            .Include(t => t.UserEntity)
            .FirstOrDefaultAsync(t => t.Token.Equals(token) && now < t.ExpiredAt && !t.IsUsed);
        if (tokenEntity == null)
        {
            controller.ViewBag.Type = "alert-danger";
            controller.ViewBag.AlertMessage = $"Podany token nie istnieje, wygasł bądź został już wykorzystany.";
            _logger.LogError("Attempt to proceed request with non existing or invalid token. Token: {}", token);
            return;
        }
        tokenEntity.IsUsed = true;

        UserEntity selectedUser = tokenEntity.UserEntity;
        selectedUser.Password = _passwordHasher.HashPassword(selectedUser, payloadDto.Dto.NewPassword);

        _context.Update(tokenEntity);
        await _context.SaveChangesAsync(); 
        
        string responseMessage = "Hasło do Twojego konta zostało pomyślnie zmienione.";
        controller.HttpContext.Session.SetString(SessionKey.CHANGE_PASSWORD_LOGIN_REDIRECT, responseMessage);
        controller.Response.Redirect("/Auth/Login");
    }
}