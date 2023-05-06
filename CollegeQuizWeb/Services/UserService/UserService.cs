using System;
using System.Collections.Generic;
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
using Microsoft.IdentityModel.Tokens;
using CouponListDto = CollegeQuizWeb.Dto.User.CouponListDto;

namespace CollegeQuizWeb.Services.UserService;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;

    public UserService(ILogger<UserService> logger, ApplicationDbContext context,
        IPasswordHasher<UserEntity> passwordHasher)
    {
        _logger = logger;
        _context = context;
        _passwordHasher = passwordHasher;
    }



    public async Task AttemptCouponRedeem(AttemptCouponRedeemPayloadDto obj)
    {
        UserController controller = obj.ControllerReference;
        var coupon = _context.Coupons.FirstOrDefault(o => o.Token.Equals(obj.Dto.Coupon));
        
        string? loggedUser = controller.HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUser.IsNullOrEmpty())
        {
            controller.Response.Redirect("/Auth/Login");
            return;
        }

        var userEntity = _context.Users.FirstOrDefault(x => x.Username.Equals(loggedUser));
        if (userEntity == null)
        {
            controller.HttpContext.Session.Remove(SessionKey.IS_USER_LOGGED);
            controller.Response.Redirect("/Auth/Login");
            return;
        }

        if (coupon != null)
        {
            if (coupon.IsUsed)
            {
                controller.HttpContext.Session
                    .SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, Lang.USED_COUPON_CODE_ERROR);
                controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE, "alert-danger");
                controller.Response.Redirect("/User/AttemptCouponRedeem");
                return;
            }

            if (DateTime.Today > coupon.ExpiringAt)
            {
                controller.HttpContext.Session
                    .SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, Lang.INACTIVE_COUPON_CODE_ERROR);
                controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE, "alert-danger");
                controller.Response.Redirect("/User/AttemptCouponRedeem");
                return;
            }

            if (controller.ModelState.IsValid)
            {
                coupon.IsUsed = true;
                
                string message = "";
                
                if (userEntity.CurrentStatusExpirationDate.AddDays(-7) > DateTime.Now)
                {
                    controller.HttpContext.Session
                        .SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, Lang.SUBSCRIPTION_IS_ACTIVE);
                    controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE,
                        "alert-danger");
                    controller.Response.Redirect("/User/AttemptCouponRedeem");
                    return;
                }
                
                ExtendSubscription.AddSubscriptionTime(controller, userEntity, coupon, ref message);
                
                _context.Update(coupon);
                _context.Update(userEntity);
                await _context.SaveChangesAsync();
                controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, message);
                controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE, "alert-success");
                controller.Response.Redirect("/User/AttemptCouponRedeem");
            }
        }
        else
        {
            controller.HttpContext.Session
                .SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, Lang.INVALID_COUPON_CODE_ERROR);
            controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE, "alert-danger");
            controller.Response.Redirect("/User/AttemptCouponRedeem");
        }
    }

    public async Task<List<CouponListDto>> GetYourCouponsList(UserController userController, string username)
    {
        var coupons =
            _context.GiftCouponsEntities
                .Include(q => q.CouponEntity)
                .Include(q => q.UserEntity)
                .Where(q => q.UserEntity.Username.Equals(username))
                .Select(o=> o.CouponEntity)
                .ToList();

        List<CouponListDto> couponListDtos = new();
        foreach (var coupon in coupons)
        {
            CouponListDto couponListDto = new();
            couponListDto.Coupon = coupon.Token;
            couponListDto.IsUsed = coupon.IsUsed;
            couponListDto.TypeOfSubscription = coupon.TypeOfSubscription;
            couponListDto.ExpiringAt = coupon.ExpiringAt;
            couponListDtos.Add(couponListDto);
        }
        return couponListDtos;
    }
    
    public async Task<ProfileDto> UserInfo(string isLogged)
    {
        var userInfo = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(isLogged));

        var subStatus = await _context.SubsciptionTypes.FirstOrDefaultAsync(s => s.SiteId.Equals(userInfo.AccountStatus));

        string userStatus;
        
        if (subStatus == null)
        {
            userStatus = "STANDARD";
        }
        else
        {
            userStatus = subStatus.Name;
        }
        
        ProfileDto UserDto = new ProfileDto()
        {
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName,
            CreatedAt = userInfo.CreatedAt,
            Email = userInfo.Email,
            AccountStatus = userStatus,
            Username = userInfo.Username,
            Password = userInfo.Password
        };

        return UserDto;
    }
    
    public async Task<EditProfileDto> GetUserData(string isLogged)
    {
        var userData = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(isLogged));
        
        EditProfileDto UserDto = new EditProfileDto()
        {
            FirstName = userData.FirstName,
            LastName = userData.LastName,
            Username = userData.Username
        };

        return UserDto;
    }

    public async Task UpdateProfile(EditProfileDtoPayload obj, string loggedUser)
    {
        UserController controller = obj.ControllerReference;
        
        var userEntity=await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(loggedUser));

        userEntity.FirstName = obj.Dto.FirstName;
        userEntity.LastName = obj.Dto.LastName;
        userEntity.Username = obj.Dto.Username;

        if (!UsernameBelongsToUser(userEntity.Id,obj.Dto.Username))
        {
            controller.ModelState.AddModelError("Username", Lang.USERNAME_ALREADY_EXIST);
        }

        if (obj.Dto.OldPassword != null)
        {
            if (_passwordHasher.VerifyHashedPassword(userEntity, userEntity.Password, obj.Dto.OldPassword) !=
                PasswordVerificationResult.Success)
            {
                controller.ModelState.AddModelError("OldPassword", Lang.INVALID_PASSWORD);
            }
        }
        else
        {
            controller.ModelState.AddModelError("OldPassword", Lang.PASSWORD_REQUIRED);
        }
        
        if (obj.Dto.NewPassword != null)
        {
            Regex passCheck = new Regex(RegexApp.MIN_ONE_UPPER_LOWER_NUM_SPEC);
            if (obj.Dto.NewPassword.Length<8||obj.Dto.NewPassword.Length>25)
            {
                controller.ModelState.AddModelError("ConfirmPassword", Lang.PASS_LEN_ERROR);
            }
            if (passCheck.IsMatch(obj.Dto.NewPassword))
            {
                if (obj.Dto.NewPassword == obj.Dto.ConfirmPassword)
                {
                    userEntity.Password = _passwordHasher.HashPassword(userEntity, obj.Dto.NewPassword);
                }
                else
                {
                    controller.ModelState.AddModelError("ConfirmPassword", Lang.DIFFERENT_PASSWORDS);
                }
            }
            else
            {
                controller.ModelState.AddModelError("ConfirmPassword", Lang.PASSWORD_REGEX_ERROR);
            }
        }

        

        if (!controller.ModelState.IsValid) return;
        
        _context.Update(userEntity);
        await _context.SaveChangesAsync();

        controller.HttpContext.Session.SetString(SessionKey.ACCOUNT_UPDATED, Lang.PROFILE_UPDATED);
        controller.Response.Redirect("/User/Profile");
    }
    
    public async Task<List<PaymentHistoryDto>> GetPaymentHistoryList(UserController userController, string username)
    {
        var userId = _context.Users.FirstOrDefault(o => o.Username.Equals(username)).Id;
        var paymentHistoryList =
            _context.SubscriptionsPaymentsHistory
                .Where(o => o.UserId.Equals(userId))
                .OrderBy(o => o.CreatedAt);

        List<PaymentHistoryDto> paymentHistoryListDtos = new();
        foreach (var payment in paymentHistoryList)
        {
            PaymentHistoryDto paymentHistoryDto = new();
            paymentHistoryDto.Id = payment.Id;
            paymentHistoryDto.TypeOfSubscription = payment.Subscription;
            int priceH = (int)payment.Price/100;
            int priceL = (int)payment.Price%100;
            String priceString = priceH + "." + priceL;
            paymentHistoryDto.Price = priceString;
            paymentHistoryDto.OrderStatus = payment.OrderStatus;
            paymentHistoryDto.Date = payment.CreatedAt;
            paymentHistoryListDtos.Add(paymentHistoryDto);
        }
        return paymentHistoryListDtos;
    }
    
    /// <summary>
    /// Method that is being used to check if username belong to logged user
    /// </summary>
    /// <param name="id">Logged user's id</param>
    /// /// <param name="username">Logged user's username</param>
    /// <returns>True or false</returns>
    bool UsernameBelongsToUser(long? id, string username)
    {
        var user = _context.Users.FirstOrDefault(o => o.Username.Equals(username));
        if (user == null)
        {
            return true;
        }
        if (user.Id == id)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}