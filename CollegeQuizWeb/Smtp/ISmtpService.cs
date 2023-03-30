using System.Threading.Tasks;

namespace CollegeQuizWeb.Smtp;

public interface ISmtpService
{
    Task<bool> SendEmailMessage<T>(UserEmailOptions<T> options) where T: AbstractSmtpViewModel;
}