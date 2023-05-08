using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.API.Services.User;

public interface IUserAPIService
{
    /// <summary>
    /// Method that get UserDetailsDto
    /// </summary>
    /// <param name="userEntity">Current user Username</param>
    /// <returns>UserDetailsDto</returns>
    UserDetailsDto GetUserDetails(UserEntity userEntity);
}