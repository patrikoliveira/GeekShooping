using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CouponAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CouponController : ControllerBase
{

    private readonly ICouponRepository repository;

    public CouponController(ICouponRepository repository)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [Authorize]
    [HttpGet("{couponCode}")]
    public async Task<ActionResult<CouponVO>> GetCouponByCouponCode(string couponCode)
    {
        var coupon = await repository.GetCouponByCouponCode(couponCode);
        if (coupon == null)
        {
            return NotFound();
        }

        return Ok(coupon);
    }
}