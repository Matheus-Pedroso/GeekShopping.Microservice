using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Xml;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GeekShopping.Web.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService) : Controller
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

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(AddToCartViewModel model)
        {
            CartViewModel cart = new()
            {
                CartHeader = new CartHeaderViewModel
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value,
                },
            };

            CartDetailViewModel cartDetail = new CartDetailViewModel()
            {
                Count = model.Count,
                ProductId = model.Id,
                Product = await productService.FindById(model.Id),
                CartHeader = cart.CartHeader
            };

            List<CartDetailViewModel> cartDetails = new List<CartDetailViewModel>();
            cartDetails.Add(cartDetail);
            cart.CartDetails = cartDetails;

            var json = JsonConvert.SerializeObject(cart);
            Console.WriteLine(json);

            var response = await cartService.AddItemToCart(cart);
            if (response != null)
                return RedirectToAction(nameof(Index));

            return View(model);
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
