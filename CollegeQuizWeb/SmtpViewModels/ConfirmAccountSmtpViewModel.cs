using CollegeQuizWeb.Smtp;

namespace CollegeQuizWeb.SmtpViewModels;

public class ConfirmAccountSmtpViewModel : AbstractSmtpViewModel
{
    public string FullName { get; set; }
    public int TokenValidTime { get; set; }
    public string ConfirmAccountLink { get; set; }
}