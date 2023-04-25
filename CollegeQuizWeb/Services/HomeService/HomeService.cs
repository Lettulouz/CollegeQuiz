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

    public async Task<bool> ChangePaymentStatus(string paymentStatus, string orderId, string subscriptionName)
    {
        bool isGift = false;

        SubscriptionPaymentHistoryEntity subscriptionPaymentHistoryEntity =
            _context.SubscriptionsPaymentsHistory
                .FirstOrDefault(obj => obj.PayuId.Equals(orderId))!;
        subscriptionPaymentHistoryEntity.OrderStatus = paymentStatus;
        _context.Update(subscriptionPaymentHistoryEntity);
        await _context.SaveChangesAsync();

        if (subscriptionName.Contains("GIFT"))
            isGift = true;
        
        if (paymentStatus.Equals("COMPLETED"))
        {
            var userId = 
                _context.SubscriptionsPaymentsHistory
                    .FirstOrDefault(o => o.PayuId.Equals(orderId))!.UserId;

            var userEntity = _context.Users.FirstOrDefault(obj => obj.Id.Equals(userId));
            var username = userEntity.Username;
            
            int typeOfSubscription = _context.SubsciptionTypes
                .FirstOrDefault(obj => subscriptionName.Contains(obj.Name))!.SiteId;
            
            if (isGift)
            {
                bool isExactTheSame = false;
                string generatedToken;
                do
                {
                    generatedToken = Utilities.GenerateOtaToken(20);
                    var token = await _context.OtaTokens
                        .FirstOrDefaultAsync(t => t.Token.Equals(generatedToken));
                    if (token != null) isExactTheSame = true;
                } while (isExactTheSame);

                CouponEntity couponEntity = new();
                couponEntity.Token = generatedToken;
                couponEntity.ExpiringAt = DateTime.Today.AddDays(30);
                couponEntity.ExtensionTime = 30;
                couponEntity.TypeOfSubscription = typeOfSubscription;
                couponEntity.CustomerName = username;

                _context.Coupons.Add(couponEntity);
                _context.SaveChanges();

                GiftCouponsEntity giftCouponsEntity = new();
                giftCouponsEntity.CouponId = couponEntity.Id;
                giftCouponsEntity.UserId = userId;
                _context.GiftCouponsEntities.Add(giftCouponsEntity);
                await _context.SaveChangesAsync();
                return true;
            }

            userEntity.CurrentStatusExpirationDate = DateTime.Now.AddDays(30);
            userEntity.AccountStatus = typeOfSubscription;
            _context.Users.Update(userEntity);
            await _context.SaveChangesAsync();
            return true;
        }
        return true;
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

        Buyer buyer = new Buyer(userEntity.Email);
        buyer.FirstName = userEntity.FirstName;
        buyer.Phone = subscriptionPaymentDto.PhoneNumber;
        buyer.LastName = userEntity.LastName;

        int forWho = subscriptionPaymentDto.ForWho;

        string forWhoStr = "";
        if (forWho.Equals(1)) forWhoStr = subscriptionTypesEntity.Name + "SELF";
        else if (forWho.Equals(2)) forWhoStr = subscriptionTypesEntity.Name + "GIFT";
        
        var products = new List<Product>(1);
        products.Add(new Product(forWhoStr, price.ToString(), "1"));
        var request = new OrderRequest(remoteIpAddress, 
            ConfigLoader.PayuClientId, "Zakup subskypcji Quizazu", "PLN", 
            price.ToString(), products);
        request.ValidityTime = "1800";
        request.Buyer = buyer;
        request.NotifyUrl = "https://dominikpiskor.pl/Home/ChangePaymentStatus";
        request.ContinueUrl = "https://dominikpiskor.pl/Home/";
        OrderResponse orderResponse = new();
        try
        {
            orderResponse = await client.PostOrderAsync(request, default(CancellationToken));

           var status= orderResponse.Status.StatusCode;
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
        subscriptionPaymentHistoryEntity.OrderStatus = "PENDING";
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

        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            ipAddress = System.Net.Dns.GetHostEntry(ipAddress).AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }

        return ipAddress.ToString();
    }
}