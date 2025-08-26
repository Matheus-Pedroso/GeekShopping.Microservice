using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model;

namespace GeekShopping.CartAPI.Config;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductVO, Product>()
                .ReverseMap();
            cfg.CreateMap<CartHeaderVO, CartHeader>()
                .ReverseMap();
            cfg.CreateMap<CartDetailVO, CartDetail>()
                .ReverseMap();
            cfg.CreateMap<CartVO, Cart>()
                .ReverseMap();
        });
        return config;
    }
}
