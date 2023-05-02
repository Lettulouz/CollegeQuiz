namespace CollegeQuizWeb.Dto.Admin;

using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;
using System.ComponentModel.DataAnnotations;

public class SubscriptionTypeDto
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Price { get; set; }
    
    public string? CurrentDiscount { get; set; }
}

public class SubscriptionTypeDtoPayload : AbstractControllerPayloader<AdminController>
{
    public SubscriptionTypeDto Dto { get; set; }
    
    public SubscriptionTypeDtoPayload(AdminController controllerReference)
        : base(controllerReference) { }
}