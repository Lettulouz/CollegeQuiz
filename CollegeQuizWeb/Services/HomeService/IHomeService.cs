using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.Home;
using CollegeQuizWeb.Entities;
using PayU.Client.Models;

namespace CollegeQuizWeb.Services.HomeService;

public interface IHomeService
{
    /// <summary>
    /// Method that get user information
    /// for Subscription view
    /// </summary>
    /// <param name="username">Current user Username</param>
    /// <param name="controller">HomeController instance</param>
    /// <returns>subscriptionPaymentDto</returns>
    Task<SubscriptionPaymentDto> GetUserData(string username, HomeController controller);
    
    /// <summary>
    /// Method that make payment for subscription
    /// </summary>
    /// <param name="subscriptionPaymentDtoPayload">SubscriptionPaymentDtoPayload with subscription payment data</param>
    Task MakePaymentForSubscription(SubscriptionPaymentDtoPayload subscriptionPaymentDtoPayload);
    
    /// <summary>
    /// Method that change payment status
    /// </summary>
    /// <param name="paymentStatus">Payment status</param>
    /// <param name="orderId">Order id</param>
    /// <param name="subscriptionName">Subscription name</param>
    /// <returns>True or false</returns>
    Task<bool> ChangePaymentStatus(string paymentStatus, string orderId, string subscriptionName);
    
    /// <summary>
    /// Method that get subscription types
    /// for Subscription view
    /// </summary>
    /// <returns>Subscription types</returns>
    Task<List<SubscriptionTypesDto>> GetSubscriptionTypes();
}