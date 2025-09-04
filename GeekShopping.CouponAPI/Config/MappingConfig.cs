using AutoMapper;
using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Model.Context;

namespace GeekShopping.CouponAPI.Config;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CouponVO, Coupon>().ReverseMap();
        });
        return config;
    }
}
