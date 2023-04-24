using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.Home;
using PayU.Client.Models;

namespace CollegeQuizWeb.Services.HomeService;

public interface IHomeService
{

    Task<SubscriptionPaymentDto> GetUserData(string username, HomeController controller);
    Task MakePaymentForSubscription(SubscriptionPaymentDtoPayload subscriptionPaymentDtoPayload);
    Task<bool> ChangePaymentStatus(string paymentStatus, string orderId, string subscriptionName);
}