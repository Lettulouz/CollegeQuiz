using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("subscription_types")]
public class SubscriptionTypesEntity : AbstractAuditableEntity
{
    [Required]
    [Column("site_id")]
    public int SiteId { get; set; }   
    
    [Required]
    [Column("name")]
    public string Name { get; set; }

    [Required]
    [Column("price", TypeName = "decimal(5,2)")]
    public Decimal Price { get; set; }

    [Column("before_discount_price",TypeName = "decimal(5,2)")]
    public Decimal? BeforeDiscountPrice { get; set; }

    [Column("current_discount")]
    public double? CurrentDiscount { get; set; }
}