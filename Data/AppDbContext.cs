using Microsoft.EntityFrameworkCore;

namespace CachingTechniques.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var products = new List<Product>();
            for (int i = 0; i < 2000; i++)
            {
                products.Add(new Product { Id = i + 1, Name = $"Product {i + 1}" });
            }

            modelBuilder.Entity<Product>().HasData(products);
            base.OnModelCreating(modelBuilder);
        }
    }
}
