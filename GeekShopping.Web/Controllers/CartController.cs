using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers;

public class CartController : Controller
{
    private readonly IProductService productService;
    private readonly ICartService cartService;
    private readonly ICouponService couponService;

    public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
    {
        this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
        this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        this.couponService = couponService ?? throw new ArgumentNullException(nameof(couponService));
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        CartViewModel response = await FindUserCart();
        return View(response);
    }

    [HttpPost]
    [ActionName("ApplyCoupon")] 
    public async Task<IActionResult> ApplyCoupon(CartViewModel model)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

        var response = await cartService.ApplyCoupon(model, token);
        if (response)
        {
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    [HttpPost]
    [ActionName("RemoveCoupon")]
    public async Task<IActionResult> RemoveCoupon ()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

        var response = await cartService.RemoveCoupon(userId, token);
        if (response)
        {
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    public async Task<IActionResult> Remove(int id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

        var response = await cartService.RemoveFromCart(id, token);
        if (response)
        {
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    { 
        return View(await FindUserCart());
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CartViewModel model)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        
        var response = await cartService.Checkout(model.CartHeader, token);
        if (response != null && response.GetType() == typeof(string)) 
        {
            TempData["Error"] = response;
            return RedirectToAction(nameof(Checkout));
        }
        else if (response != null) 
        {
            return RedirectToAction(nameof(Confirmation));
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Confirmation()
    {
        return View();
    }

    private async Task<CartViewModel> FindUserCart()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

        var response = await cartService.FindCartByUserId(userId, token);
        if (response?.CartHeader != null)
        {
            if (!string.IsNullOrEmpty(response.CartHeader.CouponCode))
            {
                var coupon = await couponService.GetCoupon(response.CartHeader.CouponCode, token);

                if (coupon?.CouponCode != null)
                {
                    response.CartHeader.DiscountAmount = coupon.DiscountAmount;
                }
            }

            foreach (var detail in response.CartDetails)
            {
                response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);
            }

            response.CartHeader.PurchaseAmount -= response.CartHeader.DiscountAmount;
        }

        return response;
    }
}