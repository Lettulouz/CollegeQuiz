using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("subscription_payment_history")]
public class SubscriptionPaymentHistoryEntity : AbstractAuditableEntity
{
    [Required]
    [ForeignKey(nameof(UserEntity))]
    [Column("user_id")]
    public long UserId { get; set; }
    
    [Required]
    [Column("payu_order_id")]
    public string PayuId { get; set; }

    [Required]
    [Column("price")]
    public Decimal Price { get; set; }
    
    [Required]
    [Column("subscription")]
    public string Subscription { get; set; }
    public virtual UserEntity UserEntity { get; set; }
}