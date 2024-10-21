using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Models;
using ShopApp.Services;
using System.Text.Json.Serialization.Metadata;

namespace ShopApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IServiceProduct _serviceProduct;

        public ProductsController(IServiceProduct serviceProduct)
        {
            _serviceProduct = serviceProduct;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _serviceProduct.ReadAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return View(await _serviceProduct.GetByIdAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ViewResult Create() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                await _serviceProduct.CreateAsync(product);
                return RedirectToAction("Index");
            }
            return BadRequest("Model is not valid");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ViewResult> Update(int id) => View(await _serviceProduct.GetByIdAsync(id));
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("Id,Name,Price,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                await _serviceProduct.UpdateAsync(product.Id, product);
                return RedirectToAction("Index");
            }
            return BadRequest("Model is not valid");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ViewResult Delete(int id) => View(id);
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int confirm) // confirm 0 || 1
        {
            if(confirm == 1)
            {
                await _serviceProduct.DeleteAsync(id);
            }
            return RedirectToAction("Index");
        }

    }
}
