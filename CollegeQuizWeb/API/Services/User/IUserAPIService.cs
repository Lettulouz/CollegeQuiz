using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.API.Services.User;

public interface IUserAPIService
{
    UserDetailsDto GetUserDetails(UserEntity userEntity);
}