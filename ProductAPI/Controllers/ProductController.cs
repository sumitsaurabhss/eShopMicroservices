using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Exceptions;
using ProductAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductDbContext _dbContext;

        public ProductController(ILogger<ProductController> logger, ProductDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: api/<ProductController>
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return _dbContext.Products;
        }

        // GET api/<ProductController>/5
        [HttpGet("{code}")]
        public async Task<ActionResult<Product>> GetByProductCode(string code)
        {
            try
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(product => product.ProductCode == code);
                if (product == null)
                {
                    throw new NotFoundException("Product", code);
                }
                else
                    return product;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"\n\n\n\n\n\n\n\n\n\nError: {ex.Message}\n\n\n\n\n\n\n\n\n\n");
                return BadRequest();
            }
        }

        // POST api/<ProductController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        // PUT api/<ProductController>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update([FromBody] Product product)
        {
            try
            {
                if (product == null)
                {
                    throw new NotFoundException("Product", "Update");
                }
                else
                {
                    _dbContext.Products.Update(product);
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error: {ex.Message}");
                return BadRequest();
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var product = await _dbContext.Products.FindAsync(id);
                if (product == null)
                {
                    throw new NotFoundException("Product", id);
                }
                else
                {
                    _dbContext.Products.Remove(product);
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error: {ex.Message}");
                return BadRequest();
            }
        }
    }
}
