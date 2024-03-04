using Gems.TestInfrastructure.Samples.EfData.Entities;

using Microsoft.EntityFrameworkCore;

namespace Gems.TestInfrastructure.Samples.EfData
{
    public class Context : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Context(DbContextOptions<Context> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Product>()
                .HasIndex(p => p.ProductCategoryId, "ix_product_product_category_id");
        }
    }
}
