using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Dto.Admin;
using CollegeQuizWeb.Services.AdminService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using CouponListDto = CollegeQuizWeb.Dto.Admin.CouponListDto;

namespace CollegeQuizWeb.Controllers;

public class AdminController : Controller
{
    /// <summary>
    /// Admin service interface
    /// </summary>
    private readonly IAdminService _adminService;
    
    /// <summary>
    /// Admin controller, contains methods that are being used on admin panel
    /// </summary>
    /// <param name="adminService">Admin service interface</param>
    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
    
    /// <summary>
    /// Method that is being used to display main admin panel view, that contains basic informations and statistics
    /// </summary>
    /// <returns>Index View</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        string? userNotExist = HttpContext.Session.GetString(SessionKey.USER_NOT_EXIST);
        HttpContext.Session.Remove(SessionKey.USER_NOT_EXIST);
        ViewBag.userNotExist = userNotExist!;

        await _adminService.GetStats(this);
        
        return View();
    }

    //====Accounts====
    
    /// <summary>
    /// Method that is being used to render add user with normal account type view
    /// </summary>
    /// <returns>Add user with normal account type View</returns>
    [HttpGet] 
    public IActionResult AddUser()
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        return View();
    } 
    
    /// <summary>
    /// Method that is being used to add user with normal type account
    /// </summary>
    /// <param name="obj">Dto with user's account data</param>
    /// <returns>Add user View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(AddUserDto obj)
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var payloadDto = new AddUserDtoPayload(this) { Dto = obj };

        if (ModelState.IsValid)
        {
            await _adminService.AddUser(payloadDto,false);
        }
        
        return View(obj);
    }
    
    /// <summary>
    /// Method that is being used to render add user with admin account type view
    /// </summary>
    /// <returns>Add user with admin account type View</returns>
    [HttpGet] 
    public IActionResult AddAdmin(){ 
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        return View();
    } 
    
    /// <summary>
    /// Method that is being used to add user with admin type account
    /// </summary>
    /// <param name="obj">Dto with user's account data</param>
    /// <returns>Add admin View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAdmin(AddUserDto obj)
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var payloadDto = new AddUserDtoPayload(this) { Dto = obj };

        if (ModelState.IsValid)
        {
            await _adminService.AddUser(payloadDto,true);
        }
        
        return View(obj);
    }
    
    /// <summary>
    /// Method that is being used to display site users with normal account type
    /// </summary>
    /// <returns>Users list View</returns>
    public async Task<IActionResult> UsersList()
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        string? userRemoved = HttpContext.Session.GetString(SessionKey.USER_REMOVED);
        HttpContext.Session.Remove(SessionKey.USER_REMOVED);
        ViewBag.userRemoved = userRemoved!;

        string? userSuspended = HttpContext.Session.GetString(SessionKey.USER_SUSPENDED);
        HttpContext.Session.Remove(SessionKey.USER_SUSPENDED);
        ViewBag.userSuspended = userSuspended!;
        
        string? mailError = HttpContext.Session.GetString(SessionKey.ADMIN_ERROR);
        HttpContext.Session.Remove(SessionKey.ADMIN_ERROR);
        ViewBag.mailError = mailError!;
        
        var userList = await _adminService.GetUsers();
        
        return View(userList);
    }
    
    /// <summary>
    /// Method that is being used to display site users with admin account type
    /// </summary>
    /// <returns>Admin list View</returns>
    public async Task<IActionResult> AdminList()
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        string? userRemoved = HttpContext.Session.GetString(SessionKey.USER_REMOVED);
        HttpContext.Session.Remove(SessionKey.USER_REMOVED);
        ViewBag.userRemoved = userRemoved!;

        string? userSuspended = HttpContext.Session.GetString(SessionKey.USER_SUSPENDED);
        HttpContext.Session.Remove(SessionKey.USER_SUSPENDED);
        ViewBag.userSuspended = userSuspended!;
        
        string? mailError = HttpContext.Session.GetString(SessionKey.ADMIN_ERROR);
        HttpContext.Session.Remove(SessionKey.ADMIN_ERROR);
        ViewBag.mailError = mailError!;
        
        var adminList = await _adminService.GetAdmins();
        
        return View(adminList);
    }

    /// <summary>
    /// Method that is being used to edit user's account data
    /// </summary>
    /// <param name="id">Id of user to edit</param>
    /// <returns>Edit user View</returns>
    [HttpGet]
    public async Task<IActionResult> EditUser([FromRoute(Name = "id")] long id)
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var user = await _adminService.GetUserData(id, this);

        return View(user);
    }
    
    /// <summary>
    /// Method that is being used to edit user's account data
    /// </summary>
    /// <param name="obj">Dto with user's account data</param>
    /// <returns>Edit user View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(AddUserDto obj)
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var payloadDto = new AddUserDtoPayload(this) { Dto = obj };

        if (ModelState.IsValid)
        {
            await _adminService.UpdateUser(payloadDto, HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED)!);
        }
        
        return View(obj);
    }
    
    /// <summary>
    /// Method that is being used to render suspend user view
    /// </summary>
    /// <param name="id">Id of user to suspend</param>
    /// <returns>Suspend user View</returns>
    [HttpGet] 
    public IActionResult SuspendUser([FromRoute(Name = "id")] long id){ 
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin == null || isUserAdmin != "True") return Redirect("/Home");
        
        return View();
    } 

    /// <summary>
    /// Method that is being used to suspend user
    /// </summary>
    /// <param name="suspendUserDto">Dto with user to suspend</param>
    /// <returns>Suspend user View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SuspendUser(SuspendUserDto suspendUserDto)
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var payloadDto = new SuspendUserDtoPayload(this) { Dto = suspendUserDto };
        await _adminService.SuspendUser(payloadDto, HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED)!);

        return View();
    }
    
    /// <summary>
    /// Method that is being used to unban user from list view
    /// </summary>
    /// <param name="list">List of users, always first id will be unbanned</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task UnbanUser(List<UserListDto> list)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        var id = list[0].Id;
        await _adminService.UnbanUser(id, this);
    }
    
    /// <summary>
    /// Method that is being used to unban user from profile view
    /// </summary>
    /// <param name="id">Id of user that is going to be unbanned</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task UnbanUserProf(long id)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        await _adminService.UnbanUser(id, this);
    }
    
    /// <summary>
    /// Method that is being used to delete user from user list view
    /// </summary>
    /// <param name="list">List of users, always first id will be deleted</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task DelUser(List<UserListDto> list)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        var id = list[0].Id;
        await _adminService.DelUser(id, this, HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED)!);
    }
    
    /// <summary>
    /// Method that is being used to delete user from profile view
    /// </summary>
    /// <param name="id">Id of user that is going to be deleted</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task DelUserProf(long id)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        await _adminService.DelUser(id, this, HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED)!);
    }
    
    /// <summary>
    /// Method that is being used to render user profile view
    /// </summary>
    /// <param name="id">Id of the user whose profile will be displayed</param>
    /// <returns>User profile View</returns>
    [HttpGet]
    public async Task<IActionResult> UserProfile([FromRoute(Name = "id")] long id)
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        string? mailError = HttpContext.Session.GetString(SessionKey.ADMIN_ERROR);
        HttpContext.Session.Remove(SessionKey.ADMIN_ERROR);
        ViewBag.mailError = mailError!;
        
        string? emailSent = HttpContext.Session.GetString(SessionKey.EMAIL_SENT);
        HttpContext.Session.Remove(SessionKey.EMAIL_SENT);
        ViewBag.emailSent = emailSent!;
        
        await _adminService.UserInfo(id, this);
        return View();
    }
    
    /// <summary>
    /// Method that is being used to resend email
    /// </summary>
    /// <param name="id">Id to which the message will be sent</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task ResendEmail(long id)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        await _adminService.ResendEmail(id, this);
    }
    
    //=======================
    
    //====Quizes====
    
    /// <summary>
    /// Method that is being used to let admins check quizes content
    /// </summary>
    /// <param name="id">Id of quiz to show</param>
    /// <returns>Quiz View</returns>
    [HttpGet]
    public async Task<IActionResult> QuizView([FromRoute(Name = "id")] long id)
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        await _adminService.QuizInfo(id, this);
        return View();
    }

    /// <summary>
    /// Method that is being used to display quizes list
    /// </summary>
    /// <returns>Quiz list View</returns>
    [HttpGet]
    public async Task<IActionResult> QuizList()
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        string? quizRemoved = HttpContext.Session.GetString(SessionKey.QUIZ_REMOVED);
        HttpContext.Session.Remove(SessionKey.QUIZ_REMOVED);
        ViewBag.quizRemoved = quizRemoved!;
        
        string? error = HttpContext.Session.GetString(SessionKey.ADMIN_ERROR);
        HttpContext.Session.Remove(SessionKey.ADMIN_ERROR);
        ViewBag.error = error!;
        
        var quizList = await _adminService.GetQuizList();
        return View(quizList);
    }
    
    /// <summary>
    /// Method that is being used to delete quiz by admin from quizes list view
    /// </summary>
    /// <param name="list">List of quizes, always first id is going to be deleted</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task DelQuiz(List<QuizListDto> list)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        var id = list[0].Id;
        await _adminService.DelQuiz(id, this);
    }
    

    
    /// <summary>
    ///  Method that is being used to delete quiz by admin from quiz view
    /// </summary>
    /// <param name="id">Id of quiz that is going to be deleted</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task DelQuizView(long id)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        await _adminService.DelQuiz(id, this);
    }
    
    
    /// <summary>
    /// Method that is being used to lock quiz by admin from quizes list view
    /// </summary>
    /// <param name="list">List of quizes, always first id is going to be locked</param>
    public async Task LockQuiz(List<QuizListDto> list)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        var id = list[0].Id;
        await _adminService.LockQuiz(id, this);
    }
    
    /// <summary>
    ///  Method that is being used to lock quiz by admin from quiz view
    /// </summary>
    /// <param name="id">Id of quiz that is going to be locked</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task LockQuizView(long id)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        await _adminService.LockQuiz(id, this);
    }
    
    /// <summary>
    /// Method that is being used to unlock quiz by admin from quizes list view
    /// </summary>
    /// <param name="list">List of quizes, always first id is going to be unlocked</param>
    public async Task UnlockQuiz(List<QuizListDto> list)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        var id = list[0].Id;
        await _adminService.UnlockQuiz(id, this);
    }
    
    /// <summary>
    ///  Method that is being used to unlock quiz by admin from quiz view
    /// </summary>
    /// <param name="id">Id of quiz that is going to be unlocked</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task UnlockQuizView(long id)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            HttpContext.Response.Redirect("/Home");
            return;
        }
        await HttpContext.Session.CommitAsync();
        await _adminService.UnlockQuiz(id, this);
    }
    
    //=======================
    
    //====Coupons====
    
    /// <summary>
    /// Method that is being used to render add coupon view
    /// </summary>
    /// <returns>Add coupon View</returns>
    [HttpGet] 
    public IActionResult AddCoupon(){ 
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        return View();
    }

    /// <summary>
    /// Method that is being used to add coupon
    /// </summary>
    /// <param name="couponDto">Dto with coupon data</param>
    /// <returns>Coupon add View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCoupon(CouponDto couponDto)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        await HttpContext.Session.CommitAsync();
        var payloadDto = new CouponDtoPayload(this) {Dto = couponDto};
        await _adminService.CreateCoupons(payloadDto);
        return View(payloadDto.Dto);
    }
    
    /// <summary>
    /// Method that is being used to deactivate coupon
    /// </summary>
    /// <param name="couponListDto">Coupons that are supposed to be deleted</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task CouponList([Bind(Prefix = "Item1")] CouponListDto couponListDto)
    {
        await HttpContext.Session.CommitAsync();
        if (!couponListDto.OneElement.IsNullOrEmpty() && couponListDto.ManyElements.IsNullOrEmpty())
        {
            await _adminService.DeleteCoupon(couponListDto.OneElement, this);
        }
        if (!couponListDto.ManyElements.IsNullOrEmpty())
        {
            await _adminService.DeleteCoupon(couponListDto.ManyElements, this);
        }
        Response.Redirect("/Admin/CouponList");
    }

    /// <summary>
    /// Method that is being used to render coupon list view
    /// </summary>
    /// <returns>Coupon list View</returns>
    public async Task<IActionResult> CouponList()
    {
        await HttpContext.Session.CommitAsync();
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var test = await _adminService.GetCoupons();
        var tuple= new Tuple<CouponListDto, IEnumerable<CouponDto>>(new CouponListDto(), test);
        
        return View(tuple);
    }

    //=======================
    
    //====Categories====
    
    // /// <summary>
    // /// Method that is being used to render add category view
    // /// </summary>
    // /// <returns>Category add View</returns>
    // [HttpGet]
    // public IActionResult AddCategory()
    // {
    //     string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
    //     if (isUserAdmin != "True") return Redirect("/Home");
    //     
    //     return View();
    // }
    
    // /// <summary>
    // /// Method that is being used to add category
    // /// </summary>
    // /// <param name="categoryListDto">Dto with category data</param>
    // /// <returns>Category add View</returns>
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> AddCategory(CategoryListDto categoryListDto)
    // {
    //     string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
    //     if (isUserAdmin != "True") return Redirect("/Home");
    //     await HttpContext.Session.CommitAsync();
    //     var payloadDto = new CategoryListDtoPayload(this) {Dto = categoryListDto};
    //
    //     if (ModelState.IsValid)
    //     {
    //         await _adminService.CreateCategory(payloadDto);
    //     }
    //
    //     return View(payloadDto.Dto);
    // }
    //
    // /// <summary>
    // /// Method that is being used to delete category by admin
    // /// </summary>
    // /// <param name="list">List of categories, always first id is going to be deleted</param>
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task DelCategory(List<CategoryListDto> list)
    // {
    //     string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
    //     if (isUserAdmin != "True")
    //     {
    //         HttpContext.Response.Redirect("/Home");
    //         return;
    //     }
    //     await HttpContext.Session.CommitAsync();
    //     var id = list[0].CategoryId;
    //     await _adminService.DelCategory(id, this);
    // }
    //
    // /// <summary>
    // /// Method that is being used to render category list View
    // /// </summary>
    // /// <returns></returns>
    // [HttpGet]
    // public async Task<IActionResult> CategoryList()
    // {
    //     await HttpContext.Session.CommitAsync();
    //     string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
    //     if (isUserAdmin != "True") return Redirect("/Home");
    //     
    //     string? categoryRemoved = HttpContext.Session.GetString(SessionKey.CATEGORY_REMOVED);
    //     HttpContext.Session.Remove(SessionKey.CATEGORY_REMOVED);
    //     ViewBag.categoryRemoved = categoryRemoved!;
    //     
    //     var categoryList = await _adminService.GetCategoryList();
    //     return View(categoryList);
    // }
    
    //=======================
    
    //====Subscription types====
    
    /// <summary>
    /// Method that is being used to render subscriptions types view
    /// </summary>
    /// <returns>Subscriptions types View</returns>
    [HttpGet]
    public async Task<IActionResult> Subscriptions()
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        string? subUpdated = HttpContext.Session.GetString(SessionKey.SUB_UPDATED);
        HttpContext.Session.Remove(SessionKey.SUB_UPDATED);
        ViewBag.subUpdated = subUpdated!;
        
        string? subError = HttpContext.Session.GetString(SessionKey.SUB_ERROR);
        HttpContext.Session.Remove(SessionKey.SUB_ERROR);
        ViewBag.subError = subError!;
        
        var subscriptionTypes = await _adminService.GetSubscriptions();
        
        return View(subscriptionTypes);
    }

    /// <summary>
    /// Method that is being used to edit subscription types
    /// </summary>
    /// <param name="sub">Dto with subscription type data</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task Subscriptions(SubscriptionTypeDto sub)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True")
        {
            Redirect("/Home");
            return; 
        }

        var payloadDto = new SubscriptionTypeDtoPayload(this) {Dto = sub};
        
        if (ModelState.IsValid)
        {
            await _adminService.UpdateSub(payloadDto);
        }
        else
        {
            HttpContext.Session.SetString(SessionKey.SUB_ERROR, Lang.SUB_ERROR);
            Response.Redirect("/Admin/Subscriptions");
        }
    }
    
    //=======================
}

    