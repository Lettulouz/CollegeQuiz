namespace CollegeQuizWeb.Dto.Admin;

using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;
using System.ComponentModel.DataAnnotations;

public class SubscriptionTypeDto
{
    [Required(ErrorMessage = Lang.USERNAME_IS_REQUIRED_ERROR)]
    public long Id { get; set; }
    
    [Required(ErrorMessage = Lang.USERNAME_IS_REQUIRED_ERROR)]
    public string Name { get; set; }
    
    [Required(ErrorMessage = Lang.USERNAME_IS_REQUIRED_ERROR)]
    public string Price { get; set; }
    
    public string? CurrentDiscount { get; set; }
    
    public string? BeforeDiscountPrice { get; set; }
}

public class SubscriptionTypeDtoPayload : AbstractControllerPayloader<AdminController>
{
    public SubscriptionTypeDto Dto { get; set; }
    
    public SubscriptionTypeDtoPayload(AdminController controllerReference)
        : base(controllerReference) { }
}