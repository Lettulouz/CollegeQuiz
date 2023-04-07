using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.ChangePassword;
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
}