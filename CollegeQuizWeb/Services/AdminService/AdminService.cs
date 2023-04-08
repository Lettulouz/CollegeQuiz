using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.Admin;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Smtp;
using CollegeQuizWeb.SmtpViewModels;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.Services.AdminService;

public class AdminService : IAdminService
{
    private readonly ILogger<AdminService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly ISmtpService _smtpService;

    public AdminService(ILogger<AdminService> logger, ApplicationDbContext context, ISmtpService smtpService,
        IPasswordHasher<UserEntity> passwordHasher)
    {
        _logger = logger;
        _context = context;
        _smtpService = smtpService;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserEntity>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task UserInfo(long id, AdminController controller)
    {
        var userInfo = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

        if (userInfo == null)
        {
            controller.HttpContext.Session.SetString(SessionKey.USER_NOT_EXIST, Lang.USER_NOT_EXIST);
            controller.Response.Redirect("/Admin");

        }
        else
        {
            controller.ViewBag.userInfo = userInfo;

            controller.ViewBag.UserQuizes = await _context.Quizes
                .Where(q => q.UserId.Equals(id)).ToListAsync();

        }
    }

    public async Task DelUser(long id, AdminController controller)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Remove(user);
            _context.SaveChanges();
            controller.HttpContext.Session.SetString(SessionKey.USER_REMOVED, Lang.USER_DELETED);
            
        }
        
        controller.Response.Redirect("/Admin/UsersList");
    }

    public async Task SuspendUser(SuspendUserDtoPayload obj)
    {
        var controller = obj.ControllerReference;
        DateTime suspendTo = obj.Dto.SuspendedTo;
        bool perm = obj.Dto.Perm;
        var id = obj.Dto.Id;

        if (perm || suspendTo != DateTime.MinValue)
        {

            if (perm)
            {
                var user=await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
                user.AccountStatus = -1;
                _context.Update(user);
                await _context.SaveChangesAsync(); 
            }else if (suspendTo != DateTime.MinValue)
            {
                var user=await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
                user.AccountStatus = -1;
                user.CurrentStatusExpirationDate = suspendTo;
                _context.Update(user);
                await _context.SaveChangesAsync(); 
            }
            controller.HttpContext.Session.SetString(SessionKey.USER_SUSPENDED, Lang.USER_SUSPENDED);
            controller.Response.Redirect("/Admin/UsersList");
            }
        else
        {
            controller.ModelState.AddModelError("Perm", Lang.BAN_ERROR);
        }
    }

    public async Task CreateCoupons(CouponDtoPayload obj)
    {
        var controller = obj.ControllerReference;
        int amount = obj.Dto.Amount;
        DateTime expiringAt = obj.Dto.ExpiringAt;
        int extensionTime = obj.Dto.ExtensionTime;
        int typeOfSubscription = obj.Dto.TypeOfSubscription;
        List<CouponEntity> listOfGeneretedCoupons = new();
        string message;
        message = string.Format(Lang.COUPONS_GENERATED_INFO_STRING, amount, expiringAt, typeOfSubscription,
            extensionTime);
        for (int i = 0; i < amount; i++)
        {
            bool isExactTheSame = false;
            string generatedToken;
            do
            {
                generatedToken = Utilities.GenerateOtaToken(20);
                var token = await _context.OtaTokens.FirstOrDefaultAsync(t => t.Token.Equals(generatedToken));
                if (token != null) isExactTheSame = true;
            } while (isExactTheSame);
            CouponEntity couponEntity = new();
            couponEntity.Token = generatedToken;
            couponEntity.ExpiringAt = expiringAt;
            couponEntity.ExtensionTime = extensionTime;
            couponEntity.TypeOfSubscription = typeOfSubscription;
            message += generatedToken;
            message += "</br>";
            listOfGeneretedCoupons.Add(couponEntity);
        }
        controller.ViewBag.GeneratedCouponsMessage = message;
        await _context.AddRangeAsync(listOfGeneretedCoupons);
        await _context.SaveChangesAsync();
        

    }
    public async Task<List<CouponEntity>> GetCoupons()
    {
        return await _context.Coupons.ToListAsync();
    }

    
}