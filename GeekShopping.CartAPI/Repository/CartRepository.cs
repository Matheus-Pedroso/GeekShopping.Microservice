using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using GeekShopping.CartAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository;

public class CartRepository(MySQLContext context, IMapper mapper) : ICartRepository
{
    public async Task<bool> ApplyCoupon(string userId, string couponCode)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ClearCart(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<CartVO> FindCartByUserId(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveCoupon(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveFromCart(long cartDetailsId)
    {
        throw new NotImplementedException();
    }

    public async Task<CartVO> SaveOrUpdateCart(CartVO vo)
    {
        Cart cart = mapper.Map<Cart>(vo);
        // Check if the product exists in the database
        var Product = await context.Products.FirstOrDefaultAsync(p => p.Id == vo.CartDetails.FirstOrDefault().ProductId);
        if (Product == null)
        {
            context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
            await context.SaveChangesAsync();
        }

        // Check if the CartHeader is null
        var cartHeader = await context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == vo.CartHeader.UserId);
        if (cartHeader == null)
        {
            // Create new CartHeader and CartDetails
            context.CartHeaders.Add(cart.CartHeader);
            await context.SaveChangesAsync();

            cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
            cart.CartDetails.FirstOrDefault().Product = null; // Avoid re-inserting the product
            context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
            await context.SaveChangesAsync();
        }
        else
        {
            var cartDetails = await context.CartDetails.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == vo.CartDetails.FirstOrDefault().ProductId && p.CartHeaderId == cartHeader.Id);
            if (cartDetails == null)
            {
                // Create new CartDetails
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null; // Avoid re-inserting the product
                context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await context.SaveChangesAsync();
            }
            else
            {
                // Update the count of the existing CartDetails
                cart.CartDetails.FirstOrDefault().Product = null; // Avoid re-inserting the product
                cart.CartDetails.FirstOrDefault().Count += cartDetails.Count;
                cart.CartDetails.FirstOrDefault().Id = cartDetails.Id;
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetails.CartHeaderId;
                context.CartDetails.Update(cart.CartDetails.FirstOrDefault());
            }
        }
        return mapper.Map<CartVO>(cart);
    }
}
