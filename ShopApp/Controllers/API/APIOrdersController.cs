using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Models;
using ShopApp.Services;
using System.Collections.Concurrent;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShopApp.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class APIOrdersController : ControllerBase
    {
        private readonly IServiceOrder _service;
        public APIOrdersController(IServiceOrder service) 
        { 
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var orders = await _service.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id) 
        {
            var order = await _service.GetOrderById(id);
            return Ok(order);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> AddOrderAsync(string id, [FromBody] ICollection<int> productsId)
        {
            var order = new Order { OrderDate = DateTime.Now, UserId = id, Summ = 0 };

            if (await _service.CreateOrder(order, productsId))
            {
                return Ok();
            }

            return BadRequest("Error with create order");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderAsync(int id, [FromBody] ICollection<int> productsId)
        {
            if (await _service.UpdateOrder(id, productsId))
            {
                return Ok();
            }
            return BadRequest("Error with update order");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderAsync(int id)
        {
            if(await _service.DeleteOrder(id))
            {
                return Ok();
            }

            return BadRequest("Error with delete order");
        }
    }
}
