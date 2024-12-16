using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OrderAPI.Models;
using OrderAPI.Services;
using System.Security.Claims;
using OrderAPI.Exceptions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMongoCollection<Order> _orderCollection;
        private readonly ILogger<OrderController> _logger;
        private readonly IMessageProducer _messageProducer;

        public OrderController(ILogger<OrderController> logger, IMessageProducer messageProducer, IMongoDatabase database)
        {
            //var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            //var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            //// var connectionString = $"mongodb://{dbHost}:27017/{dbName}";
            //var connectionString = "mongodb://localhost:27017/orderdb";

            //var mongoUrl = MongoUrl.Create(connectionString);
            //var mongoClient = new MongoClient(mongoUrl);
            //var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _orderCollection = database.GetCollection<Order>("order");
            _logger = logger;
            _messageProducer = messageProducer;
        }

        // GET: api/<OrderController>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == "Admin")
            {
                // If the user is an admin, return all orders
                return await _orderCollection.Find(Builders<Order>.Filter.Empty).ToListAsync();
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // If the user is a regular user, return orders made by that user
                return await _orderCollection.Find(o => o.UserId == userId).ToListAsync();
            }
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Order>> GetById(string id)
        {
            try
            {
                var filterDefinition = Builders<Order>.Filter.Eq(x => x.Id, id);
                var order = await _orderCollection.Find(filterDefinition).SingleOrDefaultAsync();
                if (order == null)
                {
                    throw new NotFoundException("Product Details", id);
                }
                else
                {
                    return order;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error: {ex.Message}");
                return BadRequest();
            }
        }

        // POST api/<OrderController>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create([FromBody] IEnumerable<OrderDetail> orderDetails)
        {
            Order order = new Order();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            order.UserId = userId;
            order.TotalPrice = 0;
            order.TotalItems = 0;
            order.OrderDetails = new List<OrderDetail>();
            foreach (var item in orderDetails) 
            {
                item.Status = "Processing";
                order.OrderDetails.Add(item);
                order.TotalPrice += item.Price * item.Quantity;
                order.TotalItems += item.Quantity;
            }
            order.OrderedOn = DateTime.Now;
            await _orderCollection.InsertOneAsync(order);
            _messageProducer.SendMessage(order, "PlaceOrder", "order-info-key", "OrderInfo");
            return Ok();
        }

        // PUT api/<OrderController>/5
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Update([FromBody] Order order)
        {
            try
            {
                if (order == null)
                {
                    throw new NotFoundException("Product Details", "Update");
                }
                else
                {
                    var filterDefinition = Builders<Order>.Filter.Eq(x => x.Id, order.Id);
                    await _orderCollection.ReplaceOneAsync(filterDefinition, order);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error: {ex.Message}");
                return BadRequest();
            }
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var filterDefinition = Builders<Order>.Filter.Eq(x => x.Id, id);
                if (filterDefinition == null)
                {
                    throw new NotFoundException("Product", id);
                }
                else
                {
                    await _orderCollection.DeleteOneAsync(filterDefinition);
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
