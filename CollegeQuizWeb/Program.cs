using CollegeQuizWeb.Config;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Services.HomeService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

ConfigLoader.InsertEnvProperties(builder.Configuration);
ApplicationDbContext.AddDatabaseConfiguration(builder.Services, builder.Configuration);

////////// tutaj wstrzykiwanie serwisów ////////////////////////////////////////////////////////////////////////////////

builder.Services.AddScoped<IHomeService, HomeService>();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();