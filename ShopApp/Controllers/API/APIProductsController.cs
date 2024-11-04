﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Models;
using ShopApp.Services;

namespace ShopApp.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class APIProductsController : Controller
    {
        private readonly IServiceProduct _serviceProduct;

        public APIProductsController(IServiceProduct serviceProduct)
        {
            _serviceProduct = serviceProduct;
        }

        [HttpGet]
        public async Task<JsonResult> Index()
        {
            return Json(await _serviceProduct.ReadAsync());
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> Details(int id)
        {
            return Json(await _serviceProduct.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<JsonResult> Create(Product product)
        {
            await _serviceProduct.CreateAsync(product);
            return Json(product);
        }

        [HttpPut("{id}")]
        public async Task<JsonResult> Update(int id, Product product)
        {
            product.Id = id;
            await _serviceProduct.UpdateAsync(id, product);
            return Json(product);
        }

        [HttpDelete("{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            await _serviceProduct.DeleteAsync(id);
            return Json("Product was deleted");
        }
    }
}
