using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.ChangePassword;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Services.AdminService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        ViewBag.users = await _adminService.GetUsers();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> AddCoupon()
    {
        return View();
    }
    public async Task<IActionResult> CouponList()
    {
        ViewBag.coupons = await _adminService.GetCoupons();
        return View();
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