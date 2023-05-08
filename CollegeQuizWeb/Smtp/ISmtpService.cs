using System.Threading.Tasks;

namespace CollegeQuizWeb.Smtp;

public interface ISmtpService
{
    /// <summary>
    /// Method that send email message to user's email
    /// </summary>
    /// <param name="options">Email options</param>
    /// <typeparam name="T">Current date</typeparam>
    /// <returns>True or false</returns>
    Task<bool> SendEmailMessage<T>(UserEmailOptions<T> options) where T: AbstractSmtpViewModel;
}