using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Services.Auth;

public interface IAuthAPIService
{
    Task<LoginResDto> LoginViaApi(Controller controller, LoginReqDto reqDto);
    Task<object> TestAuthenticated(Controller controller);
}