using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CollegeQuizWeb.Config;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.Home;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Utils;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PayU.Client;
using PayU.Client.Configurations;
using PayU.Client.Models;
using Sprache;

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

    public async Task Test(string test123)
    {
        SubscriptionTypesEntity test = new();
        test.Name = test123;
        test.Price = 25;
        test.CurrentDiscount = 0;
        test.SiteId = 3;
        _context.SubsciptionTypes.Add(test);
        await _context.SaveChangesAsync();
    }
    
    public async Task<OrderResponse> MakePayment()
    {
        PayUClient client = new PayUClient(ConfigLoader.PayUClientSettings);
        
        var products = new List<Product>(2);
        products.Add(new Product("Wireless mouse", "15000", "1"));
        products.Add(new Product("HDMI cable", "6000", "1"));
        var request = new OrderRequest("127.0.0.1", ConfigLoader.PayuClientId, "RTV market", "PLN", "21000", products);
        var result = await client.PostOrderAsync(request, default(CancellationToken));
        return result;
    }

    public async Task MakePaymentForSubscription(SubscriptionPaymentDtoPayload subscriptionPaymentDtoPayload)
    {
        var controller = subscriptionPaymentDtoPayload.ControllerReference;
        if (!controller.ModelState.IsValid)
        {
            return;
        }
        
        var subscriptionPaymentDto = subscriptionPaymentDtoPayload.Dto;
        UserEntity? userEntity = new();
        userEntity =  _context.Users.FirstOrDefault(obj=> obj.Username.Equals(subscriptionPaymentDto.Username));
        if (userEntity == null)
        {
            controller.Response.Redirect("/Home");
            return;
        }
        if (subscriptionPaymentDto.RememberAddressForLater)
        {
            ClientAddressEntity? checkIfAddressExists = new();
            checkIfAddressExists =
                _context.ClientsAddresses.FirstOrDefault(obj => obj.UserEntity.Equals(userEntity));

            ClientAddressEntity clientAddressEntity = new();
            clientAddressEntity.UserEntity = userEntity;
            clientAddressEntity.PhoneNumber = subscriptionPaymentDto.PhoneNumber;
            clientAddressEntity.Country = subscriptionPaymentDto.Country;
            clientAddressEntity.State = subscriptionPaymentDto.State;
            clientAddressEntity.Address1 = subscriptionPaymentDto.Address1;
            clientAddressEntity.Address2 = subscriptionPaymentDto.Address2;
            if (checkIfAddressExists != null)
                _context.ClientsAddresses.Update(clientAddressEntity);
            else
                _context.ClientsAddresses.Add(clientAddressEntity);
        }

        SubscriptionTypesEntity? subscriptionTypesEntity = new SubscriptionTypesEntity();
        subscriptionTypesEntity = _context.SubsciptionTypes
            .FirstOrDefault(obj => obj.SiteId.Equals(subscriptionPaymentDto.SubscriptionType));
        if (subscriptionTypesEntity == null)
        {
            controller.Response.Redirect("/Home");
            return;
        }
        
        
        PayUClient client = new PayUClient(ConfigLoader.PayUClientSettings);

        var remoteIpAddress = GetIpAddress(controller);
        Decimal tempDec = subscriptionTypesEntity.Price * 100;
        int price = (int)tempDec;
        
        var products = new List<Product>(1);
        products.Add(new Product(subscriptionTypesEntity.Name, price.ToString(), "1"));
        var request = new OrderRequest(remoteIpAddress, 
            ConfigLoader.PayuClientId, "Zakup subskypcji Quizazu", "PLN", 
            price.ToString(), products);
        request.NotifyUrl = "https://dominikpiskor.pl/Home/Test123/" + userEntity.Username;
        request.ContinueUrl = "https://dominikpiskor.pl/Home/";
        OrderResponse orderResponse = new();
        try
        {
            orderResponse = await client.PostOrderAsync(request, default(CancellationToken));

           var status= orderResponse.Status.StatusCode;
           controller.HttpContext.Session.SetString(SessionKey.PAYMENT_TEST, status);
        }
        catch (Exception e)
        {
            controller.Response.Redirect("/Home");
            return;
        }

        SubscriptionPaymentHistoryEntity subscriptionPaymentHistoryEntity = new();
        subscriptionPaymentHistoryEntity.UserEntity = userEntity;
        subscriptionPaymentHistoryEntity.PayuId = orderResponse.OrderId;
        subscriptionPaymentHistoryEntity.Price = price;
        subscriptionPaymentHistoryEntity.Subscription = subscriptionTypesEntity.Name;
        _context.SubscriptionsPaymentsHistory.Add(subscriptionPaymentHistoryEntity);
        await _context.SaveChangesAsync();

        controller.Response.Redirect(orderResponse.RedirectUri);
    }
    
    private string GetIpAddress(HomeController controller)
    {
        string ipAddressString = controller.Request.HttpContext.Connection.RemoteIpAddress.ToString();

        if (ipAddressString == null)
            return null;

        IPAddress ipAddress;
        IPAddress.TryParse(ipAddressString, out ipAddress);

        // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
        // This usually only happens when the browser is on the same machine as the server.
        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            ipAddress = System.Net.Dns.GetHostEntry(ipAddress).AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }

        return ipAddress.ToString();
    }
}