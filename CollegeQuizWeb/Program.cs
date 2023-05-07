using System;
using CollegeQuizWeb.API.Services.Auth;
using CollegeQuizWeb.API.Services.Quiz;
using CollegeQuizWeb.API.Services.QuizSession;
using CollegeQuizWeb.API.Services.User;
using CollegeQuizWeb.Services.HomeService;
using CollegeQuizWeb.Config;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Hubs;
using CollegeQuizWeb.Jwt;
using CollegeQuizWeb.Services.AdminService;
using CollegeQuizWeb.Services.AuthService;
using CollegeQuizWeb.Services.ChangePasswordService;
using CollegeQuizWeb.Services.PublicQuizesService;
using CollegeQuizWeb.Services.QuizService;
using CollegeQuizWeb.Services.SharedQuizesService;
using CollegeQuizWeb.Services.UserService;
using CollegeQuizWeb.Sftp;
using CollegeQuizWeb.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(120));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllersWithViews();

ConfigLoader.InsertEnvProperties(builder.Configuration);
ApplicationDbContext.AddDatabaseConfiguration(builder.Services, builder.Configuration);

////////// tutaj wstrzykiwanie serwisów ////////////////////////////////////////////////////////////////////////////////

builder.Services.AddScoped<ApplicationDbSeeder>();
builder.Services.AddScoped<ISmtpService, SmtpService>();
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddScoped<IPublicQuizesService, PublicQuizesService>();

// serwisy kontrolerów MVC
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IChangePasswordService, ChangePasswordService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISharedQuizesService, SharedQuizesService>();

// serwisy kontrolerów API
builder.Services.AddScoped<IQuizAPIService, QuizAPIService>();
builder.Services.AddScoped<IQuizSessionAPIService, QuizSessionAPIService>();
builder.Services.AddScoped<IAuthAPIService, AuthAPIService>();
builder.Services.AddScoped<IUserAPIService, UserApiService>();

// swagger, only dev
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(ConfigLoader.GetSwaggerConfiguration());

// signalR
builder.Services.AddSignalR(o =>
{
    o.MaximumReceiveMessageSize = 64000; // in bytes
});

// JWT
builder.Services.AddScoped<IJwtService, JwtService>();

// SFTP
builder.Services.AddScoped<IAsyncSftpConnector, AsyncSftpConnector>();
builder.Services.AddScoped<IAsyncSftpService, AsyncSftpService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder => corsPolicyBuilder
        .WithOrigins("https://dominikpiskor.pl", "https://quizazu.cdn.miloszgilga.pl")
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {  
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quizazu API");
    });    
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthorization();
app.UseCors();
app.UseRouting();

// signalR
app.MapHub<QuizUserSessionHub>("/quizUserSessionHub");
app.MapHub<QuizManagerSessionHub>("/quizManagerSessionHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();