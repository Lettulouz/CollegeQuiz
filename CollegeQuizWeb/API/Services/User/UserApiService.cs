using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.API.Services.User;

public class UserApiService : IUserAPIService
{
    public UserDetailsDto GetUserDetails(UserEntity userEntity)
    {
        return new()
        {
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
            Username = userEntity.Username,
            Email = userEntity.Email,
            AccountStatus = userEntity.AccountStatus,
        };
    }
}