using System;
using CollegeQuizWeb.Services.HomeService;
using CollegeQuizWeb.Config;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Services.AdminService;
using CollegeQuizWeb.Services.AuthService;
using CollegeQuizWeb.Services.ChangePasswordService;
using CollegeQuizWeb.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(25));
builder.Services.AddControllersWithViews();

ConfigLoader.InsertEnvProperties(builder.Configuration);
ApplicationDbContext.AddDatabaseConfiguration(builder.Services, builder.Configuration);

////////// tutaj wstrzykiwanie serwisów ////////////////////////////////////////////////////////////////////////////////

builder.Services.AddScoped<ApplicationDbSeeder>();
builder.Services.AddScoped<ISmtpService, SmtpService>();
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

// serwisy kontrolerów MVC
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IChangePasswordService, ChangePasswordService>();

// serwisy kontrolerów API

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();