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
    public IActionResult Index()
    {
        string? userNotExist = HttpContext.Session.GetString(SessionKey.USER_NOT_EXIST);
        HttpContext.Session.Remove(SessionKey.USER_NOT_EXIST);
        ViewBag.userNotExist = userNotExist!;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> UsersList()
    {
        string? userRemoved = HttpContext.Session.GetString(SessionKey.USER_REMOVED);
        HttpContext.Session.Remove(SessionKey.USER_REMOVED);
        ViewBag.userRemoved = userRemoved!;

        string? userSuspended = HttpContext.Session.GetString(SessionKey.USER_SUSPENDED);
        HttpContext.Session.Remove(SessionKey.USER_SUSPENDED);
        ViewBag.userSuspended = userSuspended!;
        
        ViewBag.users = await _adminService.GetUsers();
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(AddUserDto obj)
    {
        var payloadDto = new AddUserDtoPayload(this) { Dto = obj };

        if (ModelState.IsValid)
        {
            await _adminService.AddUser(payloadDto);
        }
        
        return View(obj);
    }

    [HttpGet] public IActionResult AddUser() => View();
    [HttpGet] public IActionResult AddCoupon() => View();
    [HttpGet] public IActionResult SuspendUser([FromRoute(Name = "id")] long id) => View();

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
        var test = await _adminService.GetCoupons();
        var tuple= new Tuple<CouponListDto, IEnumerable<CouponDto>>(new CouponListDto(), test);
        return View(tuple);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SuspendUser(SuspendUserDto suspendUserDto)
    {
        var payloadDto = new SuspendUserDtoPayload(this) { Dto = suspendUserDto };
        await _adminService.SuspendUser(payloadDto);

        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task UnbanUser(long id)
    {
        await _adminService.UnbanUser(id, this);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task DelUser(long id)
    {
        await _adminService.DelUser(id, this);
    }
    
    [HttpGet]
    public async Task<IActionResult> UserProfile([FromRoute(Name = "id")] long id)
    {
        await _adminService.UserInfo(id, this);
        return View();
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