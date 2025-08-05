using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class ProductController(IProductService productService) : Controller
    {
        public async Task<IActionResult> ProductIndex()
        {
            var products = await productService.FindAll();
            return View(products);
        }

        public async Task<IActionResult> ProductCreate() => View();

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var res = await productService.Create(model);
                if (res is not null)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ProductUpdate(long id)
        {
            var model = await productService.FindById(id);
            if (model is not null)
            {
                return View(model);
            }
            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> ProductUpdate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var res = await productService.Update(model);
                if (res is not null)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }
        public async Task<IActionResult> ProductDelete(long id)
        {
            var model = await productService.FindById(id);
            if (model is not null)
            {
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductModel model)
        {
            var response = await productService.Delete(model.Id);
            if (response)
            {
                return RedirectToAction(nameof(ProductIndex));
            }
            return View(model);
        }
    }
}
