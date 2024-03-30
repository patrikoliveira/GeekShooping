using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartRepository cartRepository;
    private readonly ICouponRepository couponRepository;
    private readonly IRabbitMQMessageSender rabbitMQMessageSender;

    public CartController(ICartRepository cartRepository, ICouponRepository couponRepository, IRabbitMQMessageSender rabbitMQMessageSender)
    {
        this.cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
        this.couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
        this.rabbitMQMessageSender = rabbitMQMessageSender ?? throw new ArgumentNullException(nameof(rabbitMQMessageSender));
    }

    //[Authorize]
    [HttpGet("find-cart/{id}")]
    public async Task<ActionResult<CartVO>> FindById(string id)
    {
        var cart = await cartRepository.FindCartByUserId(id);
        if (cart == null)
        {
            return NotFound();
        }

        return Ok(cart);
    }

    [HttpPost("add-cart")]
    public async Task<ActionResult<CartVO>> AddCart(CartVO vo)
    {
        var cart = await cartRepository.SaveOrUpdateCart(vo);
        if (cart == null)
        {
            return NotFound();
        }

        return Ok(cart);
    }

    [HttpPut("update-cart")]
    public async Task<ActionResult<CartVO>> UpdateCart(CartVO vo)
    {
        var cart = await cartRepository.SaveOrUpdateCart(vo);
        if (cart == null)
        {
            return NotFound();
        }

        return Ok(cart);
    }

    [HttpDelete("remove-cart/{id}")]
    public async Task<ActionResult<CartVO>> RemoveCart(int id)
    {
        var status = await cartRepository.RemoveFromCart(id);
        if (!status)
        {
            return NotFound();
        }

        return Ok(status);
    }

    [HttpPost("apply-coupon")]
    public async Task<ActionResult<CartVO>> ApplyCoupon(CartVO vo)
    {
        var status = await cartRepository.ApplyCoupon(vo.CartHeader.UserId, vo.CartHeader.CouponCode);
        if (!status)
        {
            return NotFound();
        }

        return Ok(status);
    }

    [HttpDelete("remove-coupon/{userId}")]
    public async Task<ActionResult<CartVO>> RemoveCoupon(string userId)
    {
        var status = await cartRepository.RemoveCoupon(userId);
        if (!status)
        {
            return NotFound();
        }

        return Ok(status);
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<CartVO>> Checkout(CheckoutHeaderVO vo)
    {
        string token = Request.Headers["Authorization"];
        //string token = await HttpContent.GetTokenAsync("Authorization");

        if (vo?.UserId == null)
        {
            return BadRequest();
        }

        var cart = await cartRepository.FindCartByUserId(vo.UserId);

        if (cart == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(vo.CouponCode))
        {
            CouponVO coupon = await couponRepository.GetCoupon(vo.CouponCode, token);

            if (vo.DiscountAmount != coupon.DiscountAmount)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            }
        }

        vo.CartDetails = cart.CartDetails;
        vo.DateTime = DateTime.Now;

        rabbitMQMessageSender.SendMessage(vo, "checkoutqueue");

        await cartRepository.ClearCart(vo.UserId);

        return Ok(vo); 
    }
}
