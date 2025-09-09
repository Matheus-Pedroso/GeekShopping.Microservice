using System.Net;
using System.Net.Http;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interface;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace GeekShopping.Web.Services;

public class CouponService(HttpClient httpClient) : ICouponService
{
    private readonly HttpClient _client = httpClient;
    public const string BasePath = "api/v1/Coupon";

    public async Task<CouponViewModel> GetCoupon(string couponCode)
    {
        var response = await _client.GetAsync($"{BasePath}/{couponCode}");
        if (response.StatusCode != HttpStatusCode.OK)
            return new CouponViewModel();

        return await response.ReadContentAs<CouponViewModel>();
    }
}
