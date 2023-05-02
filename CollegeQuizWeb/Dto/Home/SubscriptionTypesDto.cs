using System;

namespace CollegeQuizWeb.Dto.Home;

public class SubscriptionTypesDto
{
    public string Name { get; set; }
    public Decimal Price { get; set; }
    public double? CurrentDiscount { get; set; }
    public Decimal? PreviousPrice { get; set; }
}