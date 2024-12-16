using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using System.Text.Json;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _dbContext;

        public ProductService(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _dbContext.Products;
        }

        public async Task<Product> GetByProductCode(string code)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(product => product.ProductCode == code);
            return product;
        }

        public async Task Create([FromBody] Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return;
        }

        public async Task Update([FromBody] Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return;
        }

        public async Task Delete(Guid id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            return;
        }
    }
}
