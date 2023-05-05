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
using Microsoft.ClearScript.JavaScript;
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
        UserEntity? userEntity = _context.Users.FirstOrDefault(obj => obj.Username.Equals(username));
        if(userEntity == null){controller.Response.Redirect("/Home");return new SubscriptionPaymentDto();}

        ClientAddressEntity? clientAddressEntity =
            _context.ClientsAddresses.FirstOrDefault(x => x.UserEntity.Username.Equals(username));
        
        SubscriptionPaymentDto subscriptionPaymentDto = new SubscriptionPaymentDto();
        subscriptionPaymentDto.FirstName = userEntity.FirstName;
        subscriptionPaymentDto.LastName = userEntity.LastName;
        subscriptionPaymentDto.Email = userEntity.Email;
        subscriptionPaymentDto.Username = username;
        if (clientAddressEntity != null)
        {
            subscriptionPaymentDto.PhoneNumber = clientAddressEntity.PhoneNumber;
            subscriptionPaymentDto.Country = clientAddressEntity.Country;
            subscriptionPaymentDto.State = clientAddressEntity.State;
            subscriptionPaymentDto.Address1 = clientAddressEntity.Address1;
            subscriptionPaymentDto.Address2 = clientAddressEntity.Address2;
        }
        else
        {
            subscriptionPaymentDto.PhoneNumber = "";
            subscriptionPaymentDto.Country = "";
            subscriptionPaymentDto.State = "";
            subscriptionPaymentDto.Address1 = "";
            subscriptionPaymentDto.Address2 = "";
        }
        
        return subscriptionPaymentDto;
    }

    public async Task<bool> ChangePaymentStatus(string paymentStatus, string orderId, string subscriptionName)
    {
        bool isGift = false;

        SubscriptionPaymentHistoryEntity subscriptionPaymentHistoryEntity =
            _context.SubscriptionsPaymentsHistory
                .FirstOrDefault(obj => obj.PayuId.Equals(orderId))!;
        switch (paymentStatus)
        {
            case "PENDING":
                subscriptionPaymentHistoryEntity.OrderStatus = Lang.PAYU_PENDING;
                break;
            case "WAITING_FOR_CONFIRMATION":
                subscriptionPaymentHistoryEntity.OrderStatus = Lang.PAYU_WAITING;
                break;
            case "COMPLETED":
                subscriptionPaymentHistoryEntity.OrderStatus = Lang.PAYU_COMPLETED;
                break;
            case "CANCELED":
                subscriptionPaymentHistoryEntity.OrderStatus = Lang.PAYU_CANCELED;
                break;
        }
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

            int daysToAdd = 0;
            var calculateRemainingTime =
                (userEntity.CurrentStatusExpirationDate - DateTime.Today).TotalDays;
            if (calculateRemainingTime % 2 == 0)
                daysToAdd = (int) (calculateRemainingTime / 2);
            else
                daysToAdd = (int) (calculateRemainingTime / 2) + 1;
            userEntity.CurrentStatusExpirationDate = DateTime.Now.AddDays(30+daysToAdd);
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
            ClientAddressEntity clientAddressEntity =
                _context.ClientsAddresses.FirstOrDefault(obj => obj.UserEntity.Equals(userEntity));
            bool isAddressNull = true;
            if (clientAddressEntity != null)
                isAddressNull = false;
            else
                clientAddressEntity = new();
            clientAddressEntity.UserEntity = userEntity;
            clientAddressEntity.PhoneNumber = subscriptionPaymentDto.PhoneNumber;
            clientAddressEntity.Country = subscriptionPaymentDto.Country;
            clientAddressEntity.State = subscriptionPaymentDto.State;
            clientAddressEntity.Address1 = subscriptionPaymentDto.Address1;
            clientAddressEntity.Address2 = subscriptionPaymentDto.Address2;
            
            if (!isAddressNull)
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

        if (forWho.Equals(1) && userEntity.AccountStatus>subscriptionPaymentDto.SubscriptionType)
        {
            controller.HttpContext.Session
                .SetString(SessionKey.SUBSCRIPTION_MESSAGE, Lang.SUBSCRIPTION_IS_LOWER);
            controller.HttpContext.Session.SetString(SessionKey.SUBSCRIPTION_MESSAGE_TYPE,
                "alert-danger");
            controller.Response.Redirect("/Home/Subscription/" + subscriptionPaymentDto.SubscriptionType);
            return;
        }
        
        if (forWho.Equals(1) && 
            userEntity.AccountStatus.Equals(subscriptionPaymentDto.SubscriptionType) && 
            userEntity.CurrentStatusExpirationDate.AddDays(-7) > DateTime.Now)
        {
            controller.HttpContext.Session
                .SetString(SessionKey.SUBSCRIPTION_MESSAGE, Lang.SUBSCRIPTION_IS_ACTIVE);
            controller.HttpContext.Session.SetString(SessionKey.SUBSCRIPTION_MESSAGE_TYPE,
                "alert-danger");
            string url = subscriptionTypesEntity.SiteId.ToString();
            controller.Response.Redirect("/Home/Subscription/" + url);
            return;
        }

        var products = new List<Product>(1);
        products.Add(new Product(forWhoStr, price.ToString(), "1"));
        var request = new OrderRequest(remoteIpAddress, 
            ConfigLoader.PayuClientId, "Zakup subskypcji Quizazu", "PLN", 
            price.ToString(), products);
        request.ValidityTime = "1800";
        request.Buyer = buyer;
        request.NotifyUrl = "https://dominikpiskor.pl/Home/ChangePaymentStatus";
        if(forWhoStr.Contains("SELF"))
            request.ContinueUrl = "https://dominikpiskor.pl/User/SubscriptionAfterPaymentSelf";
        else
            request.ContinueUrl = "https://dominikpiskor.pl/User/SubscriptionAfterPaymentGift";
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
        subscriptionPaymentHistoryEntity.OrderStatus = Lang.PAYU_PENDING;
        _context.SubscriptionsPaymentsHistory.Add(subscriptionPaymentHistoryEntity);
        await _context.SaveChangesAsync();

        controller.Response.Redirect(orderResponse.RedirectUri);
    }

    public async Task<List<SubscriptionTypesDto>> GetSubscriptionTypes()
    {
        var subsciptionTypes = _context.SubsciptionTypes.ToList();
        List<SubscriptionTypesDto> subscriptionTypesDtos = new();
        foreach (var subsciptionType in subsciptionTypes)
        {
            SubscriptionTypesDto subscriptionTypesDto = new();
            subscriptionTypesDto.Name = subsciptionType.Name;
            subscriptionTypesDto.Price = subsciptionType.Price;
            subscriptionTypesDto.PreviousPrice = subsciptionType.BeforeDiscountPrice;
            subscriptionTypesDto.CurrentDiscount = subsciptionType.CurrentDiscount*100;
            subscriptionTypesDtos.Add(subscriptionTypesDto);
        }
            
        return subscriptionTypesDtos;
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