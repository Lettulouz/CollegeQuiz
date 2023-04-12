using CollegeQuizWeb.Smtp;

namespace CollegeQuizWeb.SmtpViewModels;

public class AdduserViewModel : AbstractSmtpViewModel
{
    public string FullName { get; set; }
    public int TokenValidTime { get; set; }
    public string ConfirmAccountLink { get; set; }
    
    public string Username { get; set; }
    public string Password { get; set; }
}