using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto.Home;

public class SubscriptionPaymentDto
{
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? Username { get; set; }
    public string? Email { get; set; }
    
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = Lang.INVALID_PHONE_NUMBER)]
    public string? PhoneNumber { get; set; }
    
    [Required(ErrorMessage = Lang.ADDRESS1_IS_REQUIRED)]
    [StringLength(40, ErrorMessage = Lang.ADDRESS1_TOO_LONG)]
    [MinLength(3, ErrorMessage = Lang.ADDRESS1_TOO_SHORT)]
    public string Address1 { get; set; }
    
    [StringLength(40)]
    public string? Address2 { get; set; }

    [Required(ErrorMessage = Lang.STATE_IS_REQUIRED)]
    [StringLength(40, ErrorMessage = Lang.STATE_TOO_LONG)]
    [MinLength(3, ErrorMessage = Lang.STATE_TOO_SHORT)]
    public string State { get; set; }
    
    [Required(ErrorMessage = Lang.COUNTRY_IS_REQUIRED)]
    [StringLength(40, ErrorMessage = Lang.COUNTRY_TOO_LONG)]
    [MinLength(3, ErrorMessage = Lang.COUNTRY_TOO_SHORT)]
    public string Country { get; set; }
    
    public int ForWho { get; set; }
    
    public bool RememberAddressForLater { get; set; }
    
    [Required]
    public int SubscriptionType { get; set; }
}

public class SubscriptionPaymentDtoPayload : AbstractControllerPayloader<HomeController>
{
    public SubscriptionPaymentDto Dto { get; set; }
    
    public SubscriptionPaymentDtoPayload(HomeController controllerReference)
        : base(controllerReference) { }
}