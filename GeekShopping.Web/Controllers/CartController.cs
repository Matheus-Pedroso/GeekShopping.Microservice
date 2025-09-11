using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers;

public class CartController(IProductService productService, ICartService cartService) : Controller
{
    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

        var response = await cartService.FindUserCart(userId);

        return View(response);
    }

    [HttpPost, ActionName("ApplyCoupon")]
    public async Task<IActionResult> ApplyCoupon(CartViewModel model)
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

        var response = await cartService.ApplyCoupon(model);

        if (response)
            return RedirectToAction(nameof(CartIndex));

        return View(response);
    }

    [HttpPost, ActionName("RemoveCoupon")]
    public async Task<IActionResult> RemoveCoupon()
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

        var response = await cartService.RemoveCoupon(userId);

        if (response)
            return RedirectToAction(nameof(CartIndex));

        return View(response);
    }

    public async Task<IActionResult> Remove(int id) // Remove Item
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
        var response = await cartService.RemoveFromCart(id);

        if (response)
            return RedirectToAction(nameof(CartIndex));

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

        var response = await cartService.FindUserCart(userId);

        return View(response);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CartViewModel model)
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

        var response = await cartService.Checkout(model.CartHeader);

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

    public async Task<IActionResult> Confirmation()
    {
        return View();
    }
}
