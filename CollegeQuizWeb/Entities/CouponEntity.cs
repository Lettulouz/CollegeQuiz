using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("coupons")]
public class CouponEntity : AbstractAuditableEntity
{
    [Required]
    [StringLength(20)]
    [Column("token")]
    public string Token { get; set; }
    
    [Column("is_used")]
    public bool IsUsed { get; set; }
    
    [Required]
    [Column("expiring_at")]
    public DateTime ExpiringAt { get; set; }
    
    [Required]
    [Column("extension_time")]
    public int ExtensionTime { get; set; }
    
    [Column("type_of_subscription")]
    [Required]
    public int TypeOfSubscription { get; set; }
    
    [StringLength(80)]
    [Column("customer_name")]
    public string? CustomerName { get; set; }
    
    public virtual ICollection<GiftCouponsEntity> GiftCouponsEntities { get; set; }
}