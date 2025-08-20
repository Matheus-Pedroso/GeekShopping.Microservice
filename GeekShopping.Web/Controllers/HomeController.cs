using System.Diagnostics;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IProductService productService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var products = await productService.FindAll();
            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var product = await productService.FindById(id);
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
