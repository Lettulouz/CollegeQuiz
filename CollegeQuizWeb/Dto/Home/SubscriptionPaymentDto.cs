using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;

namespace CollegeQuizWeb.Dto.Home;

public class SubscriptionPaymentDto
{
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? Username { get; set; }
    public string? Email { get; set; }
    
    [Required]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
    public string PhoneNumber { get; set; }
    
    [Required]
    [StringLength(40)]
    public string Address1 { get; set; }
    
    [Required]
    [StringLength(40)]
    public string Address2 { get; set; }

    [Required]
    [StringLength(40)]
    public string State { get; set; }
    
    [Required]
    [StringLength(40)]
    public string Country { get; set; }
}

public class SubscriptionPaymentDtoPayloader : AbstractControllerPayloader<HomeController>
{
    public SubscriptionPaymentDto Dto { get; set; }
    
    public SubscriptionPaymentDtoPayloader(HomeController controllerReference)
        : base(controllerReference) { }
}