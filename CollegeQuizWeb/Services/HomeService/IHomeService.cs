using System.Threading.Tasks;
using PayU.Client.Models;

namespace CollegeQuizWeb.Services.HomeService;

public interface IHomeService
{
    Task<OrderResponse> MakePayment();
}