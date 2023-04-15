using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("client_address")]
public class ClientAddressEntity : AbstractAuditableEntity
{
    [Required]
    [ForeignKey(nameof(UserEntity))]
    [Column("user_id")]
    public long UserId { get; set; }
    
    [Column("phone_number")]
    public string? PhoneNumber { get; set; }
    
    [Required]
    [Column("country")]
    public string Country { get; set; }
    
    [Required]
    [Column("state")]
    public string State { get; set; }
    
    [Required]
    [Column("address1")]
    public string Address1 { get; set; }
    
    [Column("address2")]
    public string? Address2 { get; set; }
    
    public virtual UserEntity UserEntity { get; set; }
}