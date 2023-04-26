using System;

namespace CollegeQuizWeb.Dto.User;

public class PaymentHistoryDto
{
    public long Id { get; set; }

    public string Price { get; set; }
    
    public string TypeOfSubscription { get; set; }
    
    public string OrderStatus { get; set; }
}