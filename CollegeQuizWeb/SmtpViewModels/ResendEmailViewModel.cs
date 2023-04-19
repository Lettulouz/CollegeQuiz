using CollegeQuizWeb.Smtp;

namespace CollegeQuizWeb.SmtpViewModels;

public class ResendEmailViewModel : AbstractSmtpViewModel
{
    public string FullName { get; set; }
    public int TokenValidTime { get; set; }
    public string ConfirmAccountLink { get; set; }
    
}