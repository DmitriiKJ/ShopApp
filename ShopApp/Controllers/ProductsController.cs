using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Models;
using System.Text.Json.Serialization.Metadata;

namespace ShopApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly UserContext _context;

        public ProductsController(UserContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return View(await _context.Products.FindAsync(id));
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
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return BadRequest("Model is not valid");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ViewResult> Update(int id) => View(await _context.Products.FindAsync(id));
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("Id,Name,Price,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
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
                _context.Remove(await _context.Products.FindAsync(id));
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

    }
}
