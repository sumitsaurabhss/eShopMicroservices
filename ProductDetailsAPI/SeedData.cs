using Microsoft.EntityFrameworkCore;
using ProductDetailsAPI.Models;

namespace ProductDetailsAPI
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ProductDetailsDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ProductDetailsDbContext>>()))
            {
                // Check if the database has been seeded
                if (context.ProductsDetails.Any())
                {
                    return; // Database has been seeded
                }

                // Seed the database with sample products
                context.ProductsDetails.AddRange(
                    new ProductDetails
                    {
                        Id = Guid.NewGuid(),
                        ProductCode = "LT1001",
                        Size = "14\"",
                        Manufacturer = "ABC Electronics",
                        Specification = "Electronics"
                    },
                    new ProductDetails
                    {
                        Id = Guid.NewGuid(),
                        ProductCode = "SP2001",
                        Size = "5.6\"",
                        Manufacturer = "XYZ Electronics",
                        Specification = "Electronics"
                    },
                    new ProductDetails
                    {
                        Id = Guid.NewGuid(),
                        ProductCode = "BK3001",
                        Size = "PocketBook",
                        Manufacturer = "RS Publishers",
                        Specification = "Books"
                    }
                );

                context.SaveChanges(); // Save changes to the database
            }
        }
    }
}