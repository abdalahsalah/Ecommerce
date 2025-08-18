using Microsoft.EntityFrameworkCore;

namespace Ecommerce_MVC_project.Models
{
    public class EFContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-B1HEDNI\\SQLEXPRESS;Database=Ecommerce_MVC_project;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
