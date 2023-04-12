using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.Admin;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Smtp;
using CollegeQuizWeb.SmtpViewModels;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();
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
                        banTime = "do " + suspendTo.ToString(CultureInfo.InvariantCulture);
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

    public async Task AddUser(AddUserDtoPayload obj)
    {
        AdminController controller = obj.ControllerReference;
        
        UserEntity userEntity = new();
        
        userEntity.FirstName = obj.Dto.FirstName;
        userEntity.LastName = obj.Dto.LastName;
        userEntity.Username = obj.Dto.Username;
        string pass="";
        
        if (UsernameExistsInDb(obj.Dto.Username))
        {
            controller.ModelState.AddModelError("Username", Lang.USERNAME_ALREADY_EXIST);
        }

        if (obj.Dto.Password == null)
        {
            pass = GenPass();
            userEntity.Password =  _passwordHasher.HashPassword(userEntity, pass);
        }
        else
        {
            Regex passCheck = new Regex(RegexApp.MIN_ONE_UPPER_LOWER_NUM_SPEC);
            if (obj.Dto.Password.Length<8||obj.Dto.Password.Length>25)
            {
                controller.ModelState.AddModelError("Password", Lang.PASS_LEN_ERROR);
            }
            if (passCheck.IsMatch(obj.Dto.Password))
            {
                pass = obj.Dto.Password;
                userEntity.Password = _passwordHasher.HashPassword(userEntity, obj.Dto.Password);
            }
            else
            {
                controller.ModelState.AddModelError("Password", Lang.USERNAME_ALREADY_EXIST);
            }
        }

        if (obj.Dto.IsAccountActivated)
        {
            if (obj.Dto.Password == null)
            {
                controller.ModelState.AddModelError("Password", Lang.PASS_REQUIRED);
            }
            userEntity.IsAccountActivated = true;
            userEntity.Email = "";
        }
        else
        {
            if (obj.Dto.Email == null)
            {
                controller.ModelState.AddModelError("Email", Lang.EMAIL_IS_REQUIRED_ERROR);
            }
            if (obj.Dto.Email?.Length>264)
            {
                controller.ModelState.AddModelError("Password", Lang.EMAIL_TOO_LONG_ERROR);
            }
            if (EmailExistsInDb(obj.Dto.Email!))
            {
                controller.ModelState.AddModelError("Email", Lang.EMAIL_ALREADY_EXIST);
            }
            if (!IsValidEmail(obj.Dto.Email!))
            {
                controller.ModelState.AddModelError("Email", Lang.EMAIL_INCORRECT_ERROR);
            }
            userEntity.Email = obj.Dto.Email!;
        }
            
        if (!controller.ModelState.IsValid) return;
        

        if (!obj.Dto.IsAccountActivated)
        {
            int tokenLife = 48; // in hours
            
            string generatedToken;
            bool isExactTheSame;
            do
            {
                generatedToken = Utilities.GenerateOtaToken();
                var token = _context.OtaTokens.FirstOrDefault(t => t.Token.Equals(generatedToken));
                isExactTheSame = (token != null);
            } while (isExactTheSame);

            OtaTokenEntity otaToken = new OtaTokenEntity()
            {
                Token = generatedToken,
                ExpiredAt = DateTime.Now.AddHours(tokenLife),
                IsUsed = false,
                UserEntity = userEntity,
            };
            _context.Add(otaToken);

            var uriBuilder = new UriBuilder(controller.Request.Scheme, controller.Request.Host.Host,
                controller.Request.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort) uriBuilder.Port = -1;
            
            AdduserViewModel emailViewModel = new()
            {
                FullName = $"{userEntity.FirstName} {userEntity.LastName}",
                TokenValidTime = tokenLife,
                ConfirmAccountLink = $"{uriBuilder.Uri.AbsoluteUri}Auth/ConfirmAccount?token={generatedToken}",
                Password = $"{pass}"
            };
            UserEmailOptions<AdduserViewModel> options = new()
            {
                TemplateName = TemplateName.CONFIRM_ACCOUNT_CREATE,
                ToEmails = new List<string>() { userEntity.Email },
                Subject = $"Tworzenie konta dla {userEntity.FirstName} {userEntity.LastName} ({userEntity.Username})",
                DataModel = emailViewModel
            };
            if (!await _smtpService.SendEmailMessage(options))
            {
               // controller.ViewBag.Type = "alert-danger";
              //  controller.ViewBag.AlertMessage =
              //      $"Nieudane wysłanie wiadomości email na adres {userEntity.Email}. Spróbuj ponownie później.";
            }
        }
        
        _context.Add(userEntity);
        _context.SaveChanges();

        String message = string.Format(Lang.USER_ADDED, userEntity.Username);
        controller.HttpContext.Session.SetString(SessionKey.USER_SUSPENDED, message);
        
        controller.Response.Redirect("/Admin/UsersList");
        
    }
    
    public async Task CreateCoupons(CouponDtoPayload obj)
    {
        var controller = obj.ControllerReference;
        int amount = obj.Dto.Amount;
        DateTime expiringAt = obj.Dto.ExpiringAt;
        int extensionTime = obj.Dto.ExtensionTime;
        int typeOfSubscription = obj.Dto.TypeOfSubscription;
        string customerName = obj.Dto.CustomerName;
        
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
            couponEntity.CustomerName = customerName;
            
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
            test3.CustomerName = VARIABLE.CustomerName;
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

    public bool EmailExistsInDb(string email)
    {
        if (_context.Users.FirstOrDefault(o => o.Email.Equals(email)) == null)
            return false;
        return true;
    }

    public bool UsernameExistsInDb(string username)
    {
        if ( _context.Users.FirstOrDefault(o => o.Username.Equals(username)) == null)
            return false;
        return true;
    }
    
    public String GenPass()
    {
        Random rand = new Random(Environment.TickCount);
        int len = rand.Next(8, 25);
        String[] charSets = new[]
        {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",
            "abcdefghijkmnopqrstuvwxyz",
            "0123456789",
            "@$!%*?&"
        };

        List<char> chars = new List<char>();

        for (int i = 0; i < charSets.Length; i++)
        {
            chars.Insert(rand.Next(0, chars.Count),
                charSets[i][rand.Next(0, charSets[i].Length)]);
        }

        for (int i = chars.Count; i < len; i++)
        {
            string rcs = charSets[rand.Next(0, charSets.Length)];
            chars.Insert(rand.Next(0, chars.Count), 
                rcs[rand.Next(0, rcs.Length)]);
        }

        return new string(chars.ToArray());
    }
    
    //https://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address
    bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith(".")) {
            return false; 
        }
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch {
            return false;
        }
    }
    
}