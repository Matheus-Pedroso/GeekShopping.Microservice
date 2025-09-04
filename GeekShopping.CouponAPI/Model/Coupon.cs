using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.CouponAPI.Model.Context;

[Table("coupon")]
public class Coupon : BaseEntity
{

    [Column("coupon_code")]
    [Required]
    [StringLength(30)]
    public string CouponCode { get; set; }

    [Column("discountamount")]
    [Required]
    public decimal DiscountAmount { get; set; }
}
