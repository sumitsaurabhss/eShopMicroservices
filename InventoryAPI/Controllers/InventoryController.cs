using InventoryAPI.Exceptions;
using InventoryAPI.Models;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly InventoryDbContext _dbContext;
        private readonly IMessageProducer _messageProducer;

        public InventoryController(ILogger<InventoryController> logger, InventoryDbContext dbContext, IMessageProducer messageProducer)
        {
            _logger = logger;
            _dbContext = dbContext;
            _messageProducer = messageProducer;
        }

        // GET: api/<InventoryController>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<Inventory>> GetInventries()
        {
            return _dbContext.Inventories;
        }

        // GET api/<InventoryController>/5
        [HttpGet("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Inventory>> GetByProductCode(string code)
        {
            try
            {
                var inventory = await _dbContext.Inventories.FirstOrDefaultAsync(inventory => inventory.ProductCode == code);
                if (inventory == null)
                {
                    throw new NotFoundException("Product Details", code);
                }
                else
                {
                    return inventory;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error: {ex.Message}");
                return BadRequest();
            }
        }

        // POST api/<InventoryController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] Inventory inventory)
        {
            await _dbContext.Inventories.AddAsync(inventory);
            await _dbContext.SaveChangesAsync();
            _messageProducer.SendMessage(inventory, "StockInformation", "stock-routing-key", "StockQueue");
            return Ok();
        }

        // PUT api/<InventoryController>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update([FromBody] Inventory inventory)
        {
            try
            {
                if (inventory == null)
                {
                    throw new NotFoundException("Product Details", "Update");
                }
                else
                {
                    _dbContext.Inventories.Update(inventory);
                    await _dbContext.SaveChangesAsync();
                    //if (inventory.Quantity == 0)
                    _messageProducer.SendMessage(inventory, "StockInformation", "stock-routing-key", "StockQueue");
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error: {ex.Message}");
                return BadRequest();
            }
        }

        // DELETE api/<InventoryController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var inventory = await _dbContext.Inventories.FindAsync(id);
                if (inventory == null)
                {
                    throw new NotFoundException("Product", id);
                }
                else
                {
                    _dbContext.Inventories.Remove(inventory);
                    await _dbContext.SaveChangesAsync();
                    _messageProducer.SendMessage(inventory, "StockInformation", "stock-routing-key", "StockQueue");
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
