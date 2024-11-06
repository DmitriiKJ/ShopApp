using Microsoft.EntityFrameworkCore;
using ShopApp.Models;

namespace ShopApp.Services
{
    public interface IServiceOrder
    {
        Task<ICollection<Order>> GetAllOrders();
        Task<Order> GetOrderById(int id);
        Task<bool> CreateOrder(Order order, ICollection<int> productsId);
        Task<bool> UpdateOrder(int id, ICollection<int> productsId);
        Task<bool> DeleteOrder(int id);

    }
    public class ServiceOrder : IServiceOrder
    {
        private readonly ProductContext _context;
        private readonly UserContext _userContext;
        public ServiceOrder(ProductContext context, UserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<bool> CreateOrder(Order order, ICollection<int> productsId)
        {
            try
            {
                var user = await _userContext.Users.FindAsync(order.UserId);
                if (order != null && user != null)
                {
                    foreach (var id in productsId)
                    {
                        var product = await _context.Products.FindAsync(id);
                        if (product != null)
                        {
                            order.Summ += product.Price;
                            order.Products.Add(product);
                        }
                    }
                    await _context.AddAsync(order);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<ICollection<Order>> GetAllOrders()
        {
            try
            {
                var res = await _context.Orders.Include(o => o.Products).ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Order> GetOrderById(int id)
        {
            try
            {
                var res = (await GetAllOrders()).FirstOrDefault(o => o.Id == id);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdateOrder(int id, ICollection<int> productsId)
        {
            try
            {
                var ord = await _context.Orders.FindAsync(id);
                if (ord != null)
                {
                    ord.OrderDate = DateTime.Now;
                    ord.Summ = 0;

                    _context.Entry(ord).Collection(o => o.Products).Load();
                    ord.Products.Clear();
                    await _context.SaveChangesAsync();

                    foreach (var idp in productsId)
                    {
                        var product = await _context.Products.FindAsync(idp);
                        if (product != null)
                        {
                            ord.Summ += product.Price;
                            ord.Products.Add(product);
                        }
                    }

                    _context.Update(ord);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
