using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.Interface;

public interface ICouponService
{
    Task<CouponViewModel> GetCoupon(string couponCode);
}
