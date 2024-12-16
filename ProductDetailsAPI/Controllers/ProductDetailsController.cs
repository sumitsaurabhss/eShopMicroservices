using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductDetailsAPI.Models;
using ProductDetailsAPI.Exceptions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductDetailsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailsController : ControllerBase
    {
        private readonly ILogger<ProductDetailsController> _logger;
        private readonly ProductDetailsDbContext _dbContext;

        public ProductDetailsController(ILogger<ProductDetailsController> logger, ProductDetailsDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: api/<ProductDetailsController>
        [HttpGet]
        public ActionResult<IEnumerable<ProductDetails>> GetProducts()
        {
            return _dbContext.ProductsDetails;
        }

        // GET api/<ProductDetailsController>/5
        [HttpGet("{code}")]
        public async Task<ActionResult<ProductDetails>> GetByProductCode(string code)
        {
            try
            {
                var productDetails = await _dbContext.ProductsDetails.FirstOrDefaultAsync(product => product.ProductCode == code);
                if (productDetails == null)
                {
                    throw new NotFoundException("Product Details", code);
                }
                else
                {
                    return productDetails;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error: {ex.Message}");
                return BadRequest();
            }
        }

        // POST api/<ProductDetailsController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] ProductDetails productDetails)
        {
            await _dbContext.ProductsDetails.AddAsync(productDetails);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        // PUT api/<ProductDetailsController>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update([FromBody] ProductDetails productDetails)
        {
            try
            {
                if (productDetails == null)
                {
                    throw new NotFoundException("Product Details", "Update");
                }
                else
                {
                    _dbContext.ProductsDetails.Update(productDetails);
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

        // DELETE api/<ProductDetailsController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var productDetails = await _dbContext.ProductsDetails.FindAsync(id);
                if (productDetails == null)
                {
                    throw new NotFoundException("Product", id);
                }
                else
                {
                    _dbContext.ProductsDetails.Remove(productDetails);
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
