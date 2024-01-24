using Microsoft.EntityFrameworkCore;
namespace passocase.Data
{
    public class OrderDbContext : DbContext
    {
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerOrderDetail> CustomerOrderDetails { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships, constraints, etc. here
        }
    }
}
