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

    public CartController(IProductService productService, ICartService cartService)
    {
        this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
        this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        CartViewModel response = await FindUserCart();
        return View(response);
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

    private async Task<CartViewModel> FindUserCart()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

        var response = await cartService.FindCartByUserId(userId, token);
        if (response?.CartHeader != null)
        {
            foreach (var detail in response.CartDetails)
            {
                response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);
            }
        }

        return response;
    }
}