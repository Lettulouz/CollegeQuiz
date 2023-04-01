using CollegeQuizWeb.Smtp;

namespace CollegeQuizWeb.SmtpViewModels;

public class ChangePasswordSmtpViewModel : AbstractSmtpViewModel
{
    public string FullName { get; set; }
    public int TokenValidTime { get; set; }
    public string ResetPasswordLink { get; set; }
}