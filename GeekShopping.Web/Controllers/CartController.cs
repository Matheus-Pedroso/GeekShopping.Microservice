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

    public async Task<IActionResult> Remove(int id) // Remove Item
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
        var response = await cartService.RemoveFromCart(id);

        if (response)
            return RedirectToAction(nameof(CartIndex));

        return View();
    }
}
