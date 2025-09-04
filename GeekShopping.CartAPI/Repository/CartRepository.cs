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
        var header = await context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

        if (header == null)
            return false;   

        header.CouponCode = couponCode;
        context.CartHeaders.Update(header);
        await context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> RemoveCoupon(string userId)
    {
        var header = await context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

        if (header == null)
            return false;

        header.CouponCode = "";
        context.CartHeaders.Update(header);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearCart(string userId)
    {
        var cartHeader = await context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cartHeader != null)
        {
            context.CartDetails.RemoveRange(context.CartDetails.Where(c => c.CartHeaderId == cartHeader.Id));
            context.CartHeaders.Remove(cartHeader);
            await context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<CartVO> FindCartByUserId(string userId)
    {
        Cart cart = new Cart();
        cart.CartHeader = await context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId);
        cart.CartDetails = context.CartDetails.AsNoTracking().Where(c => c.CartHeaderId == cart.CartHeader.Id).Include(c => c.Product);

        var cartVO = mapper.Map<CartVO>(cart);

        return cartVO;
    }
    public async Task<bool> RemoveFromCart(long cartDetailsId)
    {
        try
        {
            var cartDetails = await context.CartDetails.FirstOrDefaultAsync(c => c.Id == cartDetailsId);
            if (cartDetails == null) 
                return false;

            int totalCountOfCartItems = context.CartDetails.Count(c => c.CartHeaderId == cartDetails.CartHeaderId);

            context.CartDetails.Remove(cartDetails);
            if (totalCountOfCartItems == 1)
            {
                var cartHeaderToRemove = await context.CartHeaders.FirstOrDefaultAsync(c => c.Id == cartDetails.CartHeaderId);
                context.CartHeaders.Remove(cartHeaderToRemove);
            }
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
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
        var cartHeader = await context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == vo.CartHeader.UserId);
        if (cartHeader == null)
        {
            // Create new CartHeader and CartDetails
            context.CartHeaders.Add(cart.CartHeader);

            cart.CartDetails.FirstOrDefault().CartHeader = cart.CartHeader;
            cart.CartDetails.FirstOrDefault().Product = null; // Avoid re-inserting the product
            context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
        }
        else
        {
            var cartDetails = await context.CartDetails.FirstOrDefaultAsync(p => p.ProductId == cart.CartDetails.FirstOrDefault().ProductId && p.CartHeaderId == cartHeader.Id);
            if (cartDetails == null)
            {
                // Create new CartDetails
                var newDetail = new CartDetail
                {
                    CartHeaderId = cartHeader.Id,
                    ProductId = cart.CartDetails.First().ProductId,
                    Count = cart.CartDetails.First().Count
                };
                context.CartDetails.Add(newDetail);
            }
            else
            {
                var count = cart.CartDetails.Select(x => x.Count).FirstOrDefault();
                cartDetails.Count += count;
                cartDetails.CartHeaderId = cartDetails.CartHeaderId;
                context.CartDetails.Update(cartDetails);
            }
        }
        await context.SaveChangesAsync();

        return mapper.Map<CartVO>(cart);
    }
}
