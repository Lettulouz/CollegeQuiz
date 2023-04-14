using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.Home;
using CollegeQuizWeb.Entities;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PayU.Client;
using PayU.Client.Configurations;
using PayU.Client.Models;

namespace CollegeQuizWeb.Services.HomeService;

public class HomeService : IHomeService
{
    private readonly ILogger<HomeService> _logger;
    private readonly ApplicationDbContext _context;

    public HomeService(ILogger<HomeService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<SubscriptionPaymentDto> GetUserData(string username, HomeController controller)
    {
        UserEntity? userEntity = await _context.Users.FirstOrDefaultAsync(obj => obj.Username.Equals(username));
        if(userEntity == null){controller.Response.Redirect("/Home");return new SubscriptionPaymentDto();}

        SubscriptionPaymentDto subscriptionPaymentDto = new SubscriptionPaymentDto();
        subscriptionPaymentDto.FirstName = userEntity.FirstName;
        subscriptionPaymentDto.LastName = userEntity.LastName;
        subscriptionPaymentDto.Email = userEntity.Email;
        subscriptionPaymentDto.Username = username;
        return subscriptionPaymentDto;
    } 
    
    public async Task<OrderResponse> MakePayment()
    {
        Env.Load();
        PayUClientSettings settings = new PayUClientSettings(
            PayUContainer.PayUApiUrl.Sandbox, // Url You could use string example from configuration or use const
            "v2_1", // api version
            Env.GetString("CLIENT_ID"), // clientId from shop configuration
            Env.GetString("CLIENT_SECRET") // clientId from shop configuration
        );
        
        PayUClient client = new PayUClient(settings);
        
        var products = new List<Product>(2);
        products.Add(new Product("Wireless mouse", "15000", "1"));
        products.Add(new Product("HDMI cable", "6000", "1"));
        var request = new OrderRequest("127.0.0.1", Env.GetString("CLIENT_ID"), "RTV market", "PLN", "21000", products);
        var result = await client.PostOrderAsync(request, default(CancellationToken));
        return result;
    }
}