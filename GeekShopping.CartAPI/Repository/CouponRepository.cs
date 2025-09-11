using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model.Context;
using GeekShopping.CartAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GeekShopping.CartAPI.Repository;

public class CouponRepository(HttpClient client) : ICouponRepository
{
    private readonly HttpClient _client = client;
    public async Task<CouponVO> GetCouponByCode(string couponCode, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync($"api/v1/Coupon/{couponCode}");
        if (response.StatusCode != HttpStatusCode.OK)
            return new CouponVO();

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<CouponVO>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result;
    }
}
