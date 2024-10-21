using Microsoft.EntityFrameworkCore;
using ShopApp.Models;

namespace ShopApp.Services
{
    public interface IServiceProduct
    {
        Task<Product?> CreateAsync(Product? product);
        Task<Product?> UpdateAsync(int id, Product? product);
        Task<IEnumerable<Product>> ReadAsync();
        Task<bool> DeleteAsync(int id);
        Task<Product?> GetByIdAsync(int id);
    }

    public class ServiceProduct : IServiceProduct
    {
        private readonly ProductContext _context;
        private readonly ILogger<ServiceProduct> _logger;
        public ServiceProduct(ProductContext context, ILogger<ServiceProduct> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Product?> CreateAsync(Product? product)
        {
            if(product == null)
            {
                _logger.LogWarning("Product is null");
            }
            _ = await _context.Products.AddAsync(product);
            _ = await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);

        public async Task<IEnumerable<Product>> ReadAsync() => await _context.Products.ToListAsync();

        public async Task<Product?> UpdateAsync(int id, Product? product)
        {
            if (product == null || id != product.Id)
            {
                _logger.LogWarning("Error update");
                return null;
            }

            try
            {
                _ = _context.Products.Update(product);
                _ = await _context.SaveChangesAsync();
                return product;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
    }
}
