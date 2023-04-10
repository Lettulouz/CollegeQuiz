using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.ChangePassword;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Dto.Admin;
using CollegeQuizWeb.Services.AdminService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

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

    [HttpGet]
    public async Task<IActionResult> AddCoupon()
    {
        return View();
    }
    
   
    public async Task<IActionResult> CouponList([FromRoute(Name = "id")] string couponToDelete = "")
    {
        if (couponToDelete.Length.Equals(20))
        {
            _adminService.DeleteCoupon(couponToDelete, this);
        }
        
        var test = await _adminService.GetCoupons();
        return View(test);
    }
    
    [HttpGet]
    public async Task<IActionResult> SuspendUser([FromRoute(Name = "id")] long id)
    {
        return View();
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
    public async Task DelUser(long id)
    {

        _adminService.DelUser(id, this);
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