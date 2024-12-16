using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new InventoryDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<InventoryDbContext>>()))
            {
                // Check if the database has been seeded
                if (context.Inventories.Any())
                {
                    return; // Database has been seeded
                }

                // Seed the database with sample inventories
                context.Inventories.AddRange(
                    new Inventory
                    {
                        Id = Guid.NewGuid(),
                        ProductCode = "LT1001",
                        Quantity = 200
                    },
                    new Inventory
                    {
                        Id = Guid.NewGuid(),
                        ProductCode = "SP2001",
                        Quantity = 200
                    },
                    new Inventory
                    {
                        Id = Guid.NewGuid(),
                        ProductCode = "BK3001",
                        Quantity = 200
                    }
                );

                context.SaveChanges(); // Save changes to the database
            }
        }
    }
}