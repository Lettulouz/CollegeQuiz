using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
            String message = string.Format(Lang.USER_DELETED, user.Username);
            _context.Remove(user);
            _context.SaveChanges();
            controller.HttpContext.Session.SetString(SessionKey.USER_REMOVED, message);
            
        }
        
        controller.Response.Redirect("/Admin/UsersList");
    }

    public async Task UnbanUser(long id, AdminController controller)
    {
        var user =_context.Users.Find(id);
        if (user != null)
        {
            String message = string.Format(Lang.USER_UNBAN, user.Username);
            user.AccountStatus = 0;
            user.CurrentStatusExpirationDate = DateTime.MinValue;
            _context.Update(user);
            _context.SaveChanges();
            controller.HttpContext.Session.SetString(SessionKey.USER_REMOVED, message);
            
        }
        
        controller.Response.Redirect("/Admin/UsersList");
    }
    
    public async Task SuspendUser(SuspendUserDtoPayload obj)
    {
        var controller = obj.ControllerReference;
        DateTime suspendTo = obj.Dto.SuspendedTo;
        bool perm = obj.Dto.Perm;
        var id = obj.Dto.Id;
        var user=await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
        if (user != null)
        {
            if (perm || suspendTo != DateTime.MinValue)
            {


                    user.AccountStatus = -1;
                    String banTime = "";
                    if (perm)
                    {
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        banTime = "permanentie";
                    }
                    else if (suspendTo != DateTime.MinValue)
                    {
                        user.CurrentStatusExpirationDate = suspendTo;
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        banTime = "do " + suspendTo.ToString();
                    }

                    String message = string.Format(Lang.USER_SUSPENDED, user.Username, banTime);
                    controller.HttpContext.Session.SetString(SessionKey.USER_SUSPENDED, message);
                    controller.Response.Redirect("/Admin/UsersList");
                
            }
            else
            {
                controller.ModelState.AddModelError("Perm", Lang.BAN_ERROR);
            }
        }
        else
        {
            controller.HttpContext.Session.SetString(SessionKey.USER_NOT_EXIST, Lang.USER_NOT_EXIST);
            controller.Response.Redirect("/Admin");
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
                var token = await _context.OtaTokens
                    .FirstOrDefaultAsync(t => t.Token.Equals(generatedToken));
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
    public async Task<List<CouponDto>> GetCoupons()
    {
        var test = await _context.Coupons.ToListAsync();
        List<CouponDto> test2 = new();
        foreach (var VARIABLE in test)
        {
            CouponDto test3 = new();
            test3.Coupon = VARIABLE.Token;
            test3.ExpiringAt = VARIABLE.ExpiringAt;
            test3.TypeOfSubscription = VARIABLE.TypeOfSubscription;
            test3.ExtensionTime = VARIABLE.ExtensionTime;
            test3.IsUsed = VARIABLE.IsUsed;
            test2.Add(test3);
        }
        return test2;
    }

    public async Task DeleteCoupon(string couponsToDelete, AdminController controller)
    {
        List<string> couponsToDeleteList = new();
        if (!couponsToDelete.Contains(","))
        {
            couponsToDeleteList.Add(couponsToDelete);
        }
        else
        {
            List<string> temp = couponsToDelete.Split(',').ToList(); 
            couponsToDeleteList.AddRange(temp);
        }

        foreach (var coupon in couponsToDeleteList)
        {
            var couponEntity = _context.Coupons.FirstOrDefault(obj => obj.Token.Equals(coupon));
            if (couponEntity != null && (couponEntity.ExpiringAt > DateTime.Now))
            {
                var pastDate = DateTime.Now.AddYears(-40);
                var date = new DateTime(pastDate.Year, pastDate.Month, pastDate.Day, pastDate.Hour, pastDate.Minute,
                    pastDate.Second, pastDate.Kind);
                couponEntity.ExpiringAt = date;
                _context.Update(couponEntity);
            }
        }
        await _context.SaveChangesAsync();
        controller.Response.Redirect("/Admin/CouponList");
    }
}