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
    private readonly ICartRepository repository;
    private readonly IRabbitMQMessageSender rabbitMQMessageSender;

    public CartController(ICartRepository repository, IRabbitMQMessageSender rabbitMQMessageSender)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.rabbitMQMessageSender = rabbitMQMessageSender ?? throw new ArgumentNullException(nameof(rabbitMQMessageSender));
    }

    //[Authorize]
    [HttpGet("find-cart/{id}")]
    public async Task<ActionResult<CartVO>> FindById(string id)
    {
        var cart = await repository.FindCartByUserId(id);
        if (cart == null)
        {
            return NotFound();
        }

        return Ok(cart);
    }

    [HttpPost("add-cart")]
    public async Task<ActionResult<CartVO>> AddCart(CartVO vo)
    {
        var cart = await repository.SaveOrUpdateCart(vo);
        if (cart == null)
        {
            return NotFound();
        }

        return Ok(cart);
    }

    [HttpPut("update-cart")]
    public async Task<ActionResult<CartVO>> UpdateCart(CartVO vo)
    {
        var cart = await repository.SaveOrUpdateCart(vo);
        if (cart == null)
        {
            return NotFound();
        }

        return Ok(cart);
    }

    [HttpDelete("remove-cart/{id}")]
    public async Task<ActionResult<CartVO>> RemoveCart(int id)
    {
        var status = await repository.RemoveFromCart(id);
        if (!status)
        {
            return NotFound();
        }

        return Ok(status);
    }

    [HttpPost("apply-coupon")]
    public async Task<ActionResult<CartVO>> ApplyCoupon(CartVO vo)
    {
        var status = await repository.ApplyCoupon(vo.CartHeader.UserId, vo.CartHeader.CouponCode);
        if (!status)
        {
            return NotFound();
        }

        return Ok(status);
    }

    [HttpDelete("remove-coupon/{userId}")]
    public async Task<ActionResult<CartVO>> RemoveCoupon(string userId)
    {
        var status = await repository.RemoveCoupon(userId);
        if (!status)
        {
            return NotFound();
        }

        return Ok(status);
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<CartVO>> Checkout(CheckoutHeaderVO vo)
    {
        if (vo?.UserId == null)
        {
            return BadRequest();
        }

        var cart = await repository.FindCartByUserId(vo.UserId);

        if (cart == null)
        {
            return NotFound();
        }
        vo.CartDetails = cart.CartDetails;
        vo.DateTime = DateTime.Now;

        rabbitMQMessageSender.SendMessage(vo, "checkoutqueue");

        return Ok(vo); 
    }
}
