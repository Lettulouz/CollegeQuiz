using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.Home;
using PayU.Client.Models;

namespace CollegeQuizWeb.Services.HomeService;

public interface IHomeService
{
    Task<OrderResponse> MakePayment();
    Task<SubscriptionPaymentDto> GetUserData(string username, HomeController controller);
    Task MakePaymentForSubscription(SubscriptionPaymentDtoPayload subscriptionPaymentDtoPayload);
    Task<bool> Test(string test123);
    Task Test2();
}