using System.Threading.Tasks;
using CollegeQuizWeb.Dto.User;

namespace CollegeQuizWeb.Services.UserService;

public interface IUserService
{
    Task AttemptCouponRedeem(AttemptCouponRedeemPayloadDto obj);
}