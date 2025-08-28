using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CartController(ICartRepository cartRepository) : ControllerBase
{
    [HttpGet("find-cart/{id}")]
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

    [HttpDelete("remove-cart/{id}")]
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
}
