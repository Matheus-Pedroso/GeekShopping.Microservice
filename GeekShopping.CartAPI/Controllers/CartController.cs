using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CartController(ICartRepository cartRepository, IRabbitMQMessageSender rabbitMQMessage, ICouponRepository couponRepository) : ControllerBase
{
    [HttpGet("find-cart/{userId}")]
    public async Task<ActionResult<CartVO>> FindById(string userId)
    {
        var cart = await cartRepository.FindCartByUserId(userId);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpPost("add-cart")]
    public async Task<ActionResult<CartVO>> AddCart([FromBody] CartVO cartVO)
    {
        var cart = await cartRepository.SaveOrUpdateCart(cartVO);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpPut("update-cart")]
    public async Task<ActionResult<CartVO>> UpdateCart([FromBody] CartVO cartVO)
    {
        var cart = await cartRepository.SaveOrUpdateCart(cartVO);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpDelete("remove-cart/{cartId}")]
    public async Task<ActionResult<bool>> RemoveCart(int cartId)
    {
        var status = await cartRepository.RemoveFromCart(cartId);
        if (!status) return BadRequest();
        return Ok(status);
    }

    [HttpDelete("clear-cart/{userId}")]
    public async Task<ActionResult<bool>> ClearCart(string userId)
    {
        var status = await cartRepository.ClearCart(userId);
        if (!status) return BadRequest();
        return Ok(status);
    }

    [HttpPost("apply-coupon")]
    public async Task<ActionResult<bool>> ApplyCoupon([FromBody] CartVO cartVO)
    {
        var status = await cartRepository.ApplyCoupon(cartVO.CartHeader.UserId, cartVO.CartHeader.CouponCode);
        if (!status) return BadRequest();
        return Ok(status);
    }
    [HttpDelete("remove-coupon/{userId}")]
    public async Task<ActionResult<bool>> RemoveCoupon(string userId)
    {
        var status = await cartRepository.RemoveCoupon(userId);
        if (!status) return BadRequest();
        return Ok(status);
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<CheckoutHeaderVO>> Checkout(CheckoutHeaderVO vo)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (vo?.UserId == null) return BadRequest();
        var cart = await cartRepository.FindCartByUserId(vo.UserId);
        if (cart == null) return NotFound();
        vo.CartDetails = cart.CartDetails;
        vo.Time = DateTime.Now;
        if (!string.IsNullOrEmpty(vo.CouponCode))
        {
            CouponVO coupon = await couponRepository.GetCouponByCode(vo.CouponCode, token);
            if (vo.DiscountAmount != coupon.DiscountAmount)
                return StatusCode(412);
        }
        // Send Message to RabbitMQ
        rabbitMQMessage.SendMessage(vo, "checkoutqueue");

        return Ok(vo);
    }
}
