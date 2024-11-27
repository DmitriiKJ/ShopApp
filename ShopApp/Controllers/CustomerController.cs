using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Models;
using ShopApp.Services;

namespace ShopApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _customerService.GetAllCustomers());
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (customer != null)
            {
                if(await _customerService.CreateCustomer(customer))
                {
                    return RedirectToAction("Index");
                }
            }
            return BadRequest("Error with add customer");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return View(await _customerService.GetCustomerById(id));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(int id) => View(await _customerService.GetCustomerById(id));

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(Customer customer)
        {
            if (await _customerService.UpdateCustomer(customer.Id, customer))
            {
                return RedirectToAction("Index");
            }
            return BadRequest("Error with delete customer");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id) => View(id);

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int confirm)
        {
            if (confirm != 0)
            {
                if (await _customerService.DeleteCustomer(id))
                    return RedirectToAction("Index");
                else
                    return BadRequest("Error with delete customer");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}
