using AutoMapper;
using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Model.Context;
using GeekShopping.CouponAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Repository;

public class CouponRepository(MySQLContext context, IMapper mapper) : ICouponRepository
{
    public async Task<CouponVO> GetCouponByCode(string couponCode)
    {
        var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode);

        return mapper.Map<CouponVO>(coupon);
    }
}
