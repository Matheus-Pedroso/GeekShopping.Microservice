using System.Net.Http;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interface;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace GeekShopping.Web.Services;

public class CartService(HttpClient httpClient) : ICartService
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
    public async Task<CartViewModel> FindUserCart(string UserId)
    {
        var response = await FindCartByUserId(UserId);
        if (response?.CartHeader != null)
        {
            foreach (var detail in response.CartDetails)
            {
                response.CartHeader.PurchaseAmount += (detail.Count * detail.Product.Price); // Calcula o valor total do carrinho
            }
        }

        return response;
    }

    public async Task<bool> ApplyCoupon(CartViewModel cart, long couponCode)
    {
        throw new NotImplementedException();
    }

    public async Task<CartViewModel> Checkout(CartHeaderViewModel cartHeader)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ClearCart(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveCoupon(string userId)
    {
        throw new NotImplementedException();
    }

  
}
