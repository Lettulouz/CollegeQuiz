using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.ChangePassword;

namespace CollegeQuizWeb.Services.ChangePasswordService;

public interface IChangePasswordService
{
    Task AttemptChangePassword(AttemptChangePasswordPayloadDto payloadDto);
    Task CheckBeforeChangePassword(string token, AuthController controller);
    Task ChangePassword(string token, ChangePasswordPayloadDto payloadDto);
}