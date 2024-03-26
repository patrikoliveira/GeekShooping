using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService productService;

    public ProductController(IProductService productService)
    {
        this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    public async Task<IActionResult> ProductIndex()
    {
        var products = await productService.FindAllProducts("");
        return View(products);
    }

    public IActionResult ProductCreate()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ProductCreate(ProductModel model)
    {
        if (ModelState.IsValid)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await productService.CreateProduct(model, token);
            if (response != null)
            {
                return RedirectToAction(nameof(ProductIndex));
            }
        }

        return View(model);
    }

    public async Task<IActionResult> ProductUpdate(int id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var model = await productService.FindProductById(id, token);
        if (model != null)
        {
            return View(model);
        }
        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ProductUpdate(ProductModel model)
    {
        if (ModelState.IsValid)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await productService.UpdateProduct(model, token);
            if (response != null)
            {
                return RedirectToAction(nameof(ProductIndex));
            }
        }

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> ProductDelete(int id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var model = await productService.FindProductById(id, token);
        if (model != null)
        {
            return View(model);
        }
        return NotFound();
    }

    [Authorize(Roles = Role.Admin)]
    [HttpPost]
    public async Task<IActionResult> ProductDelete(ProductModel model)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var response = await productService.DeleteProductById(model.Id, token);
        if (response)
        {
            return RedirectToAction(nameof(ProductIndex));
        }

        return View(model);
    }
}