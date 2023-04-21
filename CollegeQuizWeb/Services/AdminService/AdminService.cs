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

 
    
    public async Task<List<UserListDto>> GetUsers()
    {
       var users = await _context.Users.Where(u=>u.IsAdmin==false).ToListAsync();
       
       List<UserListDto> DtoList = new();
       
       foreach (var userData in users)
       {
           UserListDto userModel = new();

           userModel.Id = userData.Id;
           userModel.FirstName = userData.FirstName;
           userModel.LastName = userData.LastName;
           userModel.CreatedAt = userData.CreatedAt;
           userModel.Email = userData.Email;
           userModel.UserName = userData.Username;
           userModel.AccountStatus =userData.AccountStatus;
           userModel.IsAccountActivated = userData.IsAccountActivated; 
           DtoList.Add(userModel);
       }

       return DtoList;
    }
    
    public async Task<List<UserListDto>> GetAdmins()
    {
        var users = await _context.Users.Where(u=>u.IsAdmin==true).ToListAsync();
        
        List<UserListDto> DtoList = new();
       
        foreach (var userData in users)
        {
            UserListDto userModel = new();

            userModel.Id = userData.Id;
            userModel.FirstName = userData.FirstName;
            userModel.LastName = userData.LastName;
            userModel.CreatedAt = userData.CreatedAt;
            userModel.Email = userData.Email;
            userModel.UserName = userData.Username;
            userModel.AccountStatus =userData.AccountStatus;
            userModel.IsAccountActivated = userData.IsAccountActivated; 
            DtoList.Add(userModel);
        }

        return DtoList;
    }

    public async Task GetStats(AdminController controller)
    {
        
        var adminStats = await (from u in _context.Users
            group u by 1
            into g
            select new
            {
                Total = _context.Users.Count(s => s.IsAdmin==true),
                Gold = _context.Users.Count(s => s.AccountStatus == 1 && s.IsAdmin==true),
                Platinum = _context.Users.Count(s => s.AccountStatus == 2 && s.IsAdmin==true),
                Suspended = _context.Users.Count(s => s.AccountStatus == -1 && s.IsAdmin==true)
            }).FirstOrDefaultAsync();

        controller.ViewBag.adminStats = adminStats ?? new { Total = 0, Gold = 0, Platinum = 0, Suspended = 0};
        
        var userStats = await (from u in _context.Users
            group u by 1
            into g
            select new
            {
                Total = _context.Users.Count(s => s.IsAdmin==false),
                Gold = _context.Users.Count(s => s.AccountStatus == 1 && s.IsAdmin==false),
                Platinum = _context.Users.Count(s => s.AccountStatus == 2 && s.IsAdmin==false),
                Suspended = _context.Users.Count(s => s.AccountStatus == -1 && s.IsAdmin==false)
            }).FirstOrDefaultAsync();
        
        controller.ViewBag.userStats = userStats ?? new { Total = 0, Gold = 0, Platinum = 0, Suspended = 0};
        
        var quizStats = await (from q in _context.Quizes
            group q by 1
            into g
            select new
            {
                Total = _context.Quizes.Count(),
                Public = _context.Quizes.Count(s => s.IsPublic == true),
                Private = _context.Quizes.Count(s => s.IsPublic == false)
            }).FirstOrDefaultAsync();
        
        controller.ViewBag.quizStats = quizStats ?? new { Total = 0, Public = 0, Private = 0};
        
       var couponStats = await (from q in _context.Coupons
            group q by 1
            into g
            select new
            {
                Total = _context.Coupons.Count(),
                Archive = _context.Coupons.Count(s => s.IsUsed == true || s.ExpiringAt <= DateTime.Now),
                Active = _context.Coupons.Count(s => s.IsUsed == false && s.ExpiringAt > DateTime.Now)
            }).FirstOrDefaultAsync();
       
       controller.ViewBag.couponStats = couponStats ?? new { Total = 0, Archive = 0, Active = 0};
    }
    
    
    
    //user view
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

            controller.ViewBag.payments = await _context.SubscriptionsPaymentsHistory
                .Where(p => p.UserId.Equals(id))
                .OrderByDescending(c => c.CreatedAt).ToListAsync();
        }
    }
    
   
    //User edit
    public async Task<AddUserDto> GetUserData(long id, AdminController controller)
    {
        var userInfo = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
        if (userInfo == null)
        {
            controller.HttpContext.Session.SetString(SessionKey.USER_NOT_EXIST, Lang.USER_NOT_EXIST);
            controller.Response.Redirect("/Admin");
            return new AddUserDto();
        }

        AddUserDto userEdit = new AddUserDto()
        {
            Id = userInfo.Id, FirstName =  userInfo.FirstName,
            LastName = userInfo.LastName, Username = userInfo.Username,
            Email = userInfo.Email, IsAdmin = userInfo.IsAdmin
        };
        
        return userEdit;
    }

    public async Task UpdateUser(AddUserDtoPayload obj, string loggedUser)
    {
        AdminController controller = obj.ControllerReference;
        
        long? id = obj.Dto.Id;
        string pass="niezmienione";
        
        var userEntity=await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

        if (userEntity == null)
        {
            controller.HttpContext.Session.SetString(SessionKey.USER_NOT_EXIST, Lang.USER_NOT_EXIST);
            controller.Response.Redirect("/Admin");
            return;
        }
        
        if (userEntity.Username == loggedUser)
        {
            userEntity.IsAdmin = true;
        }
        else
        {
            userEntity.IsAdmin = obj.Dto.IsAdmin;
        }
        userEntity.FirstName = obj.Dto.FirstName;
        userEntity.LastName = obj.Dto.LastName;
        userEntity.Username = obj.Dto.Username;
        userEntity.Email = obj.Dto.Email;
        
        
        if (!UsernameBelongsToUser(id,obj.Dto.Username))
        {
            controller.ModelState.AddModelError("Username", Lang.USERNAME_ALREADY_EXIST);
        }
        if (!EmailBelongsToUser(id,obj.Dto.Email!))
        {
            controller.ModelState.AddModelError("Email", Lang.EMAIL_ALREADY_EXIST);
        }
        if (!IsValidEmail(obj.Dto.Email))
        {
            controller.ModelState.AddModelError("Email", Lang.EMAIL_INCORRECT_ERROR);
        }
        if (obj.Dto.Email==null)
        {
            controller.ModelState.AddModelError("Email", Lang.EMAIL_TOO_LONG_ERROR);
            
        }
        if (obj.Dto.Email?.Length>264)
        {
            controller.ModelState.AddModelError("Password", Lang.EMAIL_TOO_LONG_ERROR);
        }

        if (obj.Dto.Password!=null)
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
                controller.ModelState.AddModelError("Password", Lang.PASSWORD_REGEX_ERROR);
            }
        }

        if (!controller.ModelState.IsValid) return;
        
        _context.Update(userEntity);
        await _context.SaveChangesAsync();
        
        AdduserViewModel emailViewModel = new()
        {
            FullName = $"{userEntity.FirstName} {userEntity.LastName}",
            Username = $"{userEntity.Username}",
            Password = $"{pass}"
        };
        UserEmailOptions<AdduserViewModel> options = new()
        {
            TemplateName = TemplateName.EDIT_USER,
            ToEmails = new List<string>() { userEntity.Email },
            Subject = $"Aktualizacja konta dla {userEntity.FirstName} {userEntity.LastName} ({userEntity.Username})",
            DataModel = emailViewModel
        };
        if (!await _smtpService.SendEmailMessage(options))
        {
            String mess = string.Format(Lang.EMAIL_SENDING_ERROR, userEntity.Email);
            controller.HttpContext.Session.SetString(SessionKey.ADMIN_ERROR, mess);
        }
        
        String message = string.Format(Lang.USER_UPDATED, userEntity.Username);
        controller.HttpContext.Session.SetString(SessionKey.USER_SUSPENDED, message);
        if (userEntity.IsAdmin)
        {
            controller.Response.Redirect("/Admin/AdminList");
        }
        else
        {
            controller.Response.Redirect("/Admin/UsersList");
        }
        
        
    }
    
    public async Task DelUser(long id, AdminController controller, string loggedUser)
    {
        var user = _context.Users.Find(id);
        bool isAdmin = false;
        if (user != null)
        {
            if (loggedUser == user.Username)
            {
                controller.HttpContext.Session.SetString(SessionKey.ADMIN_ERROR, Lang.CANNOT_DELETE_YOURSELF);
                controller.Response.Redirect("/Admin/AdminList");
                return;
            }

            
            if (user.IsAdmin)
            {
                isAdmin = true;
            }
            String message = string.Format(Lang.USER_DELETED, user.Username);
            _context.Remove(user);
            await _context.SaveChangesAsync();
            controller.HttpContext.Session.SetString(SessionKey.USER_REMOVED, message);
        }
        if (isAdmin)
        {
            controller.Response.Redirect("/Admin/AdminList");
        }
        else
        {
            controller.Response.Redirect("/Admin/UsersList");
        }
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
        if (user.IsAdmin)
        {
            controller.Response.Redirect("/Admin/AdminList");
        }
        else
        {
            controller.Response.Redirect("/Admin/UsersList");
        }
    }
    
    public async Task SuspendUser(SuspendUserDtoPayload obj, string loggedUser)
    {
        var controller = obj.ControllerReference;
        DateTime suspendTo = obj.Dto.SuspendedTo;
        bool perm = obj.Dto.Perm;
        var id = obj.Dto.Id;
        var user=await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));



        if (user != null)
        {

            if (loggedUser == user.Username)
            {
                controller.HttpContext.Session.SetString(SessionKey.ADMIN_ERROR, Lang.CANNOT_SUSPEND_YOURSELF);
                controller.Response.Redirect("/Admin/AdminList");
                return;
            }

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
                    if (user.IsAdmin)
                    {
                        controller.Response.Redirect("/Admin/AdminList");
                    }
                    else
                    {
                        controller.Response.Redirect("/Admin/UsersList");
                    }
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

    public async Task AddUser(AddUserDtoPayload obj, bool Admin)
    {
        AdminController controller = obj.ControllerReference;
        
        UserEntity userEntity = new();
        
        userEntity.FirstName = obj.Dto.FirstName;
        userEntity.LastName = obj.Dto.LastName;
        userEntity.Username = obj.Dto.Username;
        userEntity.RulesAccept = true;
        userEntity.IsAdmin = Admin;
        
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
                controller.ModelState.AddModelError("Password", Lang.PASSWORD_REGEX_ERROR);
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
            if (!IsValidEmail(obj.Dto.Email))
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
                Username = $"{userEntity.Username}",
                Password = $"{pass}"
            };
            UserEmailOptions<AdduserViewModel> options = new()
            {
                TemplateName = TemplateName.ADD_USER,
                ToEmails = new List<string>() { userEntity.Email },
                Subject = $"Tworzenie konta dla {userEntity.FirstName} {userEntity.LastName} ({userEntity.Username})",
                DataModel = emailViewModel
            };
            if (!await _smtpService.SendEmailMessage(options))
            {
                String mess = string.Format(Lang.EMAIL_SENDING_ERROR, userEntity.Email);
                controller.HttpContext.Session.SetString(SessionKey.ADMIN_ERROR, mess);
            }
        }
        
        _context.Add(userEntity);
        await _context.SaveChangesAsync();

        String message = string.Format(Lang.USER_ADDED, userEntity.Username);
        controller.HttpContext.Session.SetString(SessionKey.USER_SUSPENDED, message);
        
        if (userEntity.IsAdmin)
        {
            controller.Response.Redirect("/Admin/AdminList");
        }
        else
        {
            controller.Response.Redirect("/Admin/UsersList");
        }
        
    }

    public async Task ResendEmail(long id, AdminController controller)
    {
        var userEntity=await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
        
        if (userEntity == null)
        {
            controller.HttpContext.Session.SetString(SessionKey.USER_NOT_EXIST, Lang.USER_NOT_EXIST);
            controller.Response.Redirect("/Admin");
            return;
        }

        int tokenLife = 48; // in hours
            
        string generatedToken;
        bool isExactTheSame;
        bool isSent = true;
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
            
        ResendEmailViewModel emailViewModel = new()
        {
            FullName = $"{userEntity.FirstName} {userEntity.LastName}",
            TokenValidTime = tokenLife,
            ConfirmAccountLink = $"{uriBuilder.Uri.AbsoluteUri}Auth/ConfirmAccount?token={generatedToken}",
        };
        UserEmailOptions<ResendEmailViewModel> options = new()
        {
            TemplateName = TemplateName.RESEND_EMAIL,
            ToEmails = new List<string>() { userEntity.Email },
            Subject = $"Tworzenie konta dla {userEntity.FirstName} {userEntity.LastName} ({userEntity.Username})",
            DataModel = emailViewModel
        };
        if (!await _smtpService.SendEmailMessage(options))
        {
            String mess = string.Format(Lang.EMAIL_SENDING_ERROR, userEntity.Email);
            controller.HttpContext.Session.SetString(SessionKey.ADMIN_ERROR, mess);
            isSent = false;
        }

        if (isSent)
        {
            await _context.SaveChangesAsync();
            String mess = string.Format(Lang.EMAIL_SENT, userEntity.Email);
            controller.HttpContext.Session.SetString(SessionKey.EMAIL_SENT, mess);
        }
            controller.Response.Redirect("/Admin/UserProfile/"+userEntity.Id);
    }


    public async Task<List<QuizListDto>> GetQuizList()
    {
        var quizes = await _context.Quizes.Include(u=>u.UserEntity).ToListAsync();

        List<QuizListDto> DtoList = new();

        foreach (var quizData in quizes)
        {
            QuizListDto quizModel = new();

            quizModel.Id = quizData.Id;
            quizModel.Name = quizData.Name;
            quizModel.IsPublic = quizData.IsPublic;
            quizModel.CreatedAt = quizData.CreatedAt;
            quizModel.UserId = quizData.UserId;
            quizModel.UserName = quizData.UserEntity.Username;
            
            DtoList.Add(quizModel);
        }

        return DtoList;
    }
    
    public async Task DelQuiz(long id, AdminController controller)
    {
        var quiz = _context.Quizes.Find(id);
        if (quiz != null)
        {
            String message = string.Format(Lang.QUIZ_REMOVED, quiz.Name);
            _context.Remove(quiz);
            await _context.SaveChangesAsync();
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_REMOVED, message);
        }
        controller.Response.Redirect("/Admin/QuizList");
    }

    public async Task QuizInfo(long id, AdminController controller)
    {
        var quizInfo = await _context.Quizes.Include(u=>u.UserEntity)
            .FirstOrDefaultAsync(q => q.Id.Equals(id));

        if (quizInfo == null)
        {
            controller.HttpContext.Session.SetString(SessionKey.USER_NOT_EXIST, Lang.QUIZ_NOT_EXIST);
            controller.Response.Redirect("/Admin");
        }
        else
        {
            controller.ViewBag.quizInfo = quizInfo;

            controller.ViewBag.questions = await _context.Answers
                .Include(q => q.QuestionEntity)
                .Where(q => q.QuestionEntity.QuizId.Equals(quizInfo.Id))
                .Select(q => new
                {
                    question = q.QuestionEntity.Name,
                    answer = q.Name,
                    time_min = q.QuestionEntity.TimeMin,
                    time_sec = q.QuestionEntity.TimeSec
                })
                .GroupBy(q=>q.question)
                .Select(q=>new
                {
                    question = q.Key,
                    answers = q.Select(a => a.answer).ToList(),
                    time_sec = q.Sum(a=>a.time_min*60+a.time_sec)
                }).ToListAsync();
            //controller.ViewBag.UserQuizes = await _context.Quizes
            //    .Where(q => q.UserId.Equals(id)).ToListAsync();
        }
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
    bool IsValidEmail(string? email)
    {
        if (email == null)
        {
            return false;             
        }
        
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

    bool EmailBelongsToUser(long? id, string email)
    {
        var user = _context.Users.FirstOrDefault(o => o.Email.Equals(email));
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