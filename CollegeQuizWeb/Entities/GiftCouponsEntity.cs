using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("gift_coupons")]
public class GiftCouponsEntity : AbstractAuditableEntity
{
    [Column("coupon_id")]
    public long CouponId { get; set; }
    
    [Column("user_id")]
    public long UserId { get; set; }
    
    public virtual CouponEntity CouponEntity { get; set; }
    public virtual UserEntity UserEntity { get; set; }
}