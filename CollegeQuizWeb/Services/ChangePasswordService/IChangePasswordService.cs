using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.ChangePassword;

namespace CollegeQuizWeb.Services.ChangePasswordService;

public interface IChangePasswordService
{
    /// <summary>
    /// Method that make attept to change user's password
    /// </summary>
    /// <param name="payloadDto">AttemptChangePasswordPayloadDto with user login or email data</param>
    Task AttemptChangePassword(AttemptChangePasswordPayloadDto payloadDto);
    
    /// <summary>
    /// Method that check password before change
    /// </summary>
    /// <param name="token">token</param>
    /// <param name="controller">AuthController instance</param>
    Task CheckBeforeChangePassword(string token, AuthController controller);
    
    /// <summary>
    /// Method that change user's password
    /// </summary>
    /// <param name="token">token</param>
    /// <param name="payloadDto">ChangePasswordPayloadDto with user password data</param>
    Task ChangePassword(string token, ChangePasswordPayloadDto payloadDto);
}