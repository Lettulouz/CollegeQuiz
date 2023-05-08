using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Services.Auth;

public interface IAuthAPIService
{
    /// <summary>
    /// Method that login user.
    /// If the user is not found, it sets the HTTP status code of the controller's response to 401 (Unauthorized).
    /// </summary>
    /// <param name="controller">AuthAPIController Instance</param>
    /// <param name="reqDto">LoginReqDto</param>
    /// <returns>LoginResDto with true IsGood status and token if user is found otherwise false IsGood status</returns>
    Task<LoginResDto> LoginViaApi(Controller controller, LoginReqDto reqDto);
    
    /// <summary>
    /// Method that check if user is authenticated using JSON web token
    /// </summary>
    /// <param name="controller">AuthAPIController Instance</param>
    /// <returns>Authentication result</returns>
    Task<object> TestAuthenticated(Controller controller);
}