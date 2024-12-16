using CartAPI.Models;
using InventoryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CartAPI.Exceptions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly CartDbContext _dbContext;

        public CartController(ILogger<CartController> logger, CartDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: api/<CartController>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _dbContext.Carts.Where(cart => cart.UserId == userId).ToListAsync();
        }

        // POST api/<CartController>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create([FromBody] Cart cart)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            cart.UserId = userId;
            await _dbContext.Carts.AddAsync(cart);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        // PUT api/<CartController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Cart cart)
        {
            _dbContext.Carts.Update(cart);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<CartController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            var cart = await _dbContext.Carts.FindAsync(id);
            _dbContext.Carts.Remove(cart);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
