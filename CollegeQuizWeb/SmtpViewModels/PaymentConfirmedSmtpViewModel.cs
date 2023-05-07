using CollegeQuizWeb.Smtp;

namespace CollegeQuizWeb.SmtpViewModels;

public class PaymentConfirmedSmtpViewModel : AbstractSmtpViewModel
{
    public string FullName { get; set; }
    
    public string SubscriptionType { get; set; }
    
    public string AmountOfDays { get; set; }
}