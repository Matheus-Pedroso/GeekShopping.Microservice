using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CouponAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CouponController(ICouponRepository couponRepository) : ControllerBase
{
    [HttpGet("{couponCode}")]
    [Authorize]
    public async Task<ActionResult<CouponVO>> GetCouponByCode(string couponCode)
    {
        var coupon = await couponRepository.GetCouponByCode(couponCode);
        if (coupon == null) return NotFound();
        return Ok(coupon);
    }
}
