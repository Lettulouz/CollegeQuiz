using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Dto.Admin;
using CollegeQuizWeb.Services.AdminService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.IdentityModel.Tokens;
using CouponListDto = CollegeQuizWeb.Dto.Admin.CouponListDto;

namespace CollegeQuizWeb.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
    
    // GET
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        string? userNotExist = HttpContext.Session.GetString(SessionKey.USER_NOT_EXIST);
        HttpContext.Session.Remove(SessionKey.USER_NOT_EXIST);
        ViewBag.userNotExist = userNotExist!;

        await _adminService.GetStats(this);
        
        return View();
    }

 
    
    public async Task<IActionResult> UsersList()
    {
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
    
    public async Task<IActionResult> AdminList()
    {
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

    [HttpGet]
    public async Task<IActionResult> EditUser([FromRoute(Name = "id")] long id)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var user = await _adminService.GetUserData(id, this);

        return View(user);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(AddUserDto obj)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var payloadDto = new AddUserDtoPayload(this) { Dto = obj };

        if (ModelState.IsValid)
        {
            await _adminService.UpdateUser(payloadDto, HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED));
        }
        
        return View(obj);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(AddUserDto obj)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var payloadDto = new AddUserDtoPayload(this) { Dto = obj };

        if (ModelState.IsValid)
        {
            await _adminService.AddUser(payloadDto,false);
        }
        
        return View(obj);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAdmin(AddUserDto obj)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var payloadDto = new AddUserDtoPayload(this) { Dto = obj };

        if (ModelState.IsValid)
        {
            await _adminService.AddUser(payloadDto,true);
        }
        
        return View(obj);
    }

    [HttpGet] public IActionResult AddUser()
    { 
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        return View();
    } 
    
    [HttpGet] public IActionResult AddAdmin(){ 
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        return View();
    } 
    [HttpGet] public IActionResult AddCoupon(){ 
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        return View();
    } 
    [HttpGet] public IActionResult SuspendUser([FromRoute(Name = "id")] long id){ 
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        return View();
    } 

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task CouponList([Bind(Prefix = "Item1")] CouponListDto couponListDto)
    {
        
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
    
    public async Task<IActionResult> CouponList()
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var test = await _adminService.GetCoupons();
        var tuple= new Tuple<CouponListDto, IEnumerable<CouponDto>>(new CouponListDto(), test);
        return View(tuple);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SuspendUser(SuspendUserDto suspendUserDto)
    {

        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        var payloadDto = new SuspendUserDtoPayload(this) { Dto = suspendUserDto };
        await _adminService.SuspendUser(payloadDto, HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED));

        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task UnbanUser(List<UserListDto> list)
    {
        var id = list[0].Id;
        await _adminService.UnbanUser(id, this);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task UnbanUserProf(long id)
    {
        await _adminService.UnbanUser(id, this);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task ResendEmail(long id)
    {
        await _adminService.ResendEmail(id, this);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task DelUser(List<UserListDto> list)
    {
        var id = list[0].Id;
        await _adminService.DelUser(id, this, HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task DelUserProf(long id)
    {
        await _adminService.DelUser(id, this, HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED));
    }
    
    [HttpGet]
    public async Task<IActionResult> UserProfile([FromRoute(Name = "id")] long id)
    {
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
    
    [HttpGet]
    public async Task<IActionResult> QuizView([FromRoute(Name = "id")] long id)
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        await _adminService.QuizInfo(id, this);
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> QuizList()
    {
        string? isUserAdmin = HttpContext.Session.GetString(SessionKey.IS_USER_ADMIN);
        if (isUserAdmin != "True") return Redirect("/Home");
        
        string? quizRemoved = HttpContext.Session.GetString(SessionKey.QUIZ_REMOVED);
        HttpContext.Session.Remove(SessionKey.QUIZ_REMOVED);
        ViewBag.quizRemoved = quizRemoved!;
        
        var quizList = await _adminService.GetQuizList();
        return View(quizList);
    }
    
 
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task DelQuiz(List<QuizListDto> list)
    {
        var id = list[0].Id;

        await _adminService.DelQuiz(id, this);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task DelQuizView(long id)
    {

        await _adminService.DelQuiz(id, this);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCoupon(CouponDto couponDto)
    {
        var payloadDto = new CouponDtoPayload(this) {Dto = couponDto};
        await _adminService.CreateCoupons(payloadDto);
        return View(payloadDto.Dto);
    }
}