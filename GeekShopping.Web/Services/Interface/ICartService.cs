using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.Interface;

public interface ICartService
{
    Task<CartViewModel> FindCartByUserId(string userId);
    Task<CartViewModel> AddItemToCart(CartViewModel cart);
    Task<CartViewModel> UpdateCart(CartViewModel cart);
    Task<bool> RemoveFromCart(long cartDetailsId);
    Task<bool> ApplyCoupon(CartViewModel cart, long couponCode);
    Task<bool> RemoveCoupon(string userId);
    Task<bool> ClearCart(string userId);
    Task<CartViewModel> Checkout(CartHeaderViewModel cartHeader);
}
