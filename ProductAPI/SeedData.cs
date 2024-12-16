using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductAPI.Models;

namespace ProductAPI
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ProductDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ProductDbContext>>()))
            {
                // Check if the database has been seeded
                if (context.Products.Any())
                {
                    return; // Database has been seeded
                }

                // Seed the database with sample products
                context.Products.AddRange(
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Laptop",
                        ProductCode = "LT1001",
                        Price = 999.99m,
                        Category = "Electronics",
                        Image = "https://cdn3.vectorstock.com/i/1000x1000/42/57/abstract-creative-laptop-isometric-template-3d-vector-31074257.jpg",
                        Stock = true
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Smartphone",
                        ProductCode = "SP2001",
                        Price = 599.99m,
                        Category = "Electronics",
                        Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQypYOrT-8oP-av4lS5EQhXhXPynVYhX6WfPnUbMTk0Hb4wwU-AfNhCy8PxZA&s",
                        Stock = true
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Book",
                        ProductCode = "BK3001",
                        Price = 19.99m,
                        Category = "Books",
                        Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSchRAVh1n6-LE-tg62BnRNrVaEAmKDjft0F2co1Swyof0wefTEkV6ZPtJazA&s",
                        Stock = true
                    }
                );

                context.SaveChanges(); // Save changes to the database
            }
        }
    }
}