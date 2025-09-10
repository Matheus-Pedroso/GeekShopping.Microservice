using System.Net.Http;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interface;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace GeekShopping.Web.Services;

public class CartService(HttpClient httpClient, ICouponService couponService) : ICartService
{
    private readonly HttpClient _client = httpClient;
    public const string BasePath = "api/v1/Cart";

    public async Task<CartViewModel> FindCartByUserId(string userId)
    {
        var response = await _client.GetAsync($"{BasePath}/find-cart/{userId}");
        return await response.ReadContentAs<CartViewModel>();
    }
    public async Task<CartViewModel> AddItemToCart(CartViewModel cart)
    {
        var response = await _client.PostAsJson($"{BasePath}/add-cart", cart);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            throw new Exception("Something went wrong when calling API");
        }
        return await response.ReadContentAs<CartViewModel>();
    }

    public async Task<CartViewModel> UpdateCart(CartViewModel cart)
    {
        var response = await _client.PutAsJson($"{BasePath}/update-cart", cart);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Something went wrong when calling API");

        return await response.ReadContentAs<CartViewModel>();
    }

    public async Task<bool> RemoveFromCart(long cartDetailsId)
    {
        var response = await _client.DeleteAsync($"{BasePath}/remove-cart/{cartDetailsId}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Something went wrong when calling API");

        return true;
    }
    public async Task<bool> ApplyCoupon(CartViewModel cart)
    {
        //var couponValid = await couponService.GetCoupon(cart.CartHeader.CouponCode); // Valida se coupon existe
        //if (couponValid.Id == 0 || couponValid.DiscountAmount == 0)
        //    return false;

        var response = await _client.PostAsJson($"{BasePath}/apply-coupon", cart);
        if (!response.IsSuccessStatusCode)
        { 
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            throw new Exception("Something went wrong when calling API");
        }

        return true;
    }
    public async Task<bool> RemoveCoupon(string userId)
    {
        var response = await _client.DeleteAsync($"{BasePath}/remove-coupon/{userId}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Something went wrong when calling API");

        return true;
    }

    public async Task<CartViewModel> FindUserCart(string UserId)
    {
        var response = await FindCartByUserId(UserId);
        if (response?.CartHeader != null)
        {
            if (!string.IsNullOrEmpty(response.CartHeader.CouponCode))
            {
                var coupon = await couponService.GetCoupon(response.CartHeader.CouponCode);
                if (coupon?.CouponCode != null)
                    response.CartHeader.DiscountAmount = coupon.DiscountAmount;
            }
            foreach (var detail in response.CartDetails)
            {
                response.CartHeader.PurchaseAmount += (detail.Count * detail.Product.Price); // Calcula o valor total do carrinho
            }
            response.CartHeader.PurchaseAmount -= response.CartHeader.DiscountAmount; // Aplica o desconto, se houver
        }

        return response;
    }
    public async Task<CartHeaderViewModel> Checkout(CartHeaderViewModel model)
    {
        var response = await _client.PostAsJson($"{BasePath}/checkout", model);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            throw new Exception("Something went wrong when calling API");
        }

        return await response.ReadContentAs<CartHeaderViewModel>();
    }

    public async Task<bool> ClearCart(string userId)
    {
        throw new NotImplementedException();
    }
}
