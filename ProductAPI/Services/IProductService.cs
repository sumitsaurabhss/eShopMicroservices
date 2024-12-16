using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;

namespace ProductAPI.Services
{
    public interface IProductService
    {
        Task Create([FromBody] Product product);
        Task Delete(Guid id);
        Task<Product> GetByProductCode(string code);
        IEnumerable<Product> GetProducts();
        Task Update([FromBody] Product product);
    }
}