using Microsoft.AspNetCore.Mvc;
using ShopApp.Models;
using ShopApp.Services;
using System.Security.Claims;

namespace ShopApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly IServiceOrder _service;
        private readonly IServiceProduct _serviceProduct;
        private readonly ProductContext _context;
        public OrderController(IServiceOrder serviceOrder, IServiceProduct serviceProduct, ProductContext context) 
        {
            _service = serviceOrder;
            _serviceProduct = serviceProduct;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllOrders());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return View(await _service.GetOrderById(id));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UserId = userId;
            return View(await _serviceProduct.ReadAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(string userId)
        {
            var order = new Order { OrderDate = DateTime.Now, UserId = userId, Summ = 0 };
            await _service.CreateOrder(order, ViewBag.OrdersId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.OrderId = id;
            ViewBag.ProductsId = new List<int>();
            foreach (var op in _context.OrderProducts.Where(po => po.OrderId == id))
            {
                for (int i = 0; i < op.Quantity; i++)
                {
                    ViewBag.ProductsId.Add(op.ProductId);
                }
            }
            return View(await _serviceProduct.ReadAsync());
        }

        //[HttpPost]
        //public async Task<IActionResult> Update(int id, string h)
        //{
        //    await _service.UpdateOrder(id, new List<int>());
        //    return RedirectToAction("Index");
        //}

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(id);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int confirm)
        {
            if (confirm != 0)
            {
                await _service.DeleteOrder(id);
            }
            return RedirectToAction("Index");
        }
    }
}
