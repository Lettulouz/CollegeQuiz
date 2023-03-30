using System.Collections.Generic;

namespace CollegeQuizWeb.Smtp;

public class UserEmailOptions<T> where T: AbstractSmtpViewModel
{
    public string TemplateName { get; set; } 
    public List<string> ToEmails { get; set; }
    public string Subject { get; set; }
    public T DataModel { get; set; }
}