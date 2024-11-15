
using Microsoft.EntityFrameworkCore;
using ShopApp.Models;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Products)
            .WithMany(p => p.Orders)
            .UsingEntity<OrderProduct>(
                op => op.HasOne(op => op.Product).WithMany().HasForeignKey(op => op.ProductId),
                op => op.HasOne(op => op.Order).WithMany().HasForeignKey(op => op.OrderId),
                op => op.Property(op => op.Quantity).HasDefaultValue(1)
            );
    }

}