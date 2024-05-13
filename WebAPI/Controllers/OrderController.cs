using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities;
using WebApi.Handlers;
using WebApi.Models;
using WebApi.Statics;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderHandler _orderHandler;
        private readonly DateHandler _dateHandler;
        
        public OrderController(ApplicationDbContext context, OrderHandler orderHandler, DateHandler dateHandler)
        {
            _context = context;
            _orderHandler = orderHandler;
            _dateHandler = dateHandler;
        }
        
        [HttpPost]
        [Roles(Roles.CUSTOMER, Roles.ADMIN, Roles.EMPLOYEE)]
        public async Task<ActionResult> Add(string code, ProductOrderModel[] productOrders)
        {
            var userId = GetRequestUserId();
            if (userId == null)
                return BadRequest("Geen gebruiker gevonden");

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (user == null)
                return BadRequest("Geen gebruiker gevonden");

            var reservation = await _context.Reservations.FirstOrDefaultAsync(reservation => reservation.Code == code);
            if (reservation == null)
                return BadRequest("Geen reservering gevonden");

            var order = new Order()
            {
                Price = await _orderHandler.GetPriceForProductOrders(productOrders),
                UserId = userId,
                ReservationId = reservation.Id,
                ProductOrders = productOrders.Select(productOrder => new ProductOrder()
                {
                    ProductId = productOrder.ProductId,
                    Amount = productOrder.Amount,
                }).ToList(),
                TimeOrdered = DateTime.Now
            };

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        public async Task<ActionResult<Order[]>> GetAll()
        {
            return Ok(
                await _context.Orders
                .Where(order => !order.IsDelivered)
                .Include(order => order.Reservation)
                .ThenInclude(reservation => reservation.Lanes)
                .ToArrayAsync());
        }

        [HttpGet("Delivered")]
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        public async Task<ActionResult<Order[]>> GetDeliveredOrders()
        {
            var orders = await _context.Orders
                .Where(order => order.IsDelivered)
                .Include(order => order.Reservation)
                .ThenInclude(reservation => reservation.Lanes)
                .ToArrayAsync();

            return Ok(orders.Where(order => _dateHandler.GetUniqueDayKey(order.TimeOrdered) == _dateHandler.GetUniqueDayKey(DateTime.Now)));
        }

        [HttpPatch("ProductOrder/{id}")]
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        public async Task<ActionResult> SetProductOrderReady(int id, bool state)
        {
            var productOrder = await _context.ProductOrders.FirstOrDefaultAsync(productOrder => productOrder.Id == id);

            if (productOrder == null)
                return NotFound("Bestelling niet gevonden");

            productOrder.IsReady = state;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("Order/{id}")]
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        public async Task<ActionResult> SetOrderReady(int id, bool state)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(order => order.Id == id);

            if (order == null)
                return NotFound("Bestelling niet gevonden");

            order.IsReady = state;
            if (!state)
            {
                order.IsDelivered = false;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("Order/{id}")]
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        public async Task<ActionResult> SetOrderDelivered(int id, bool state)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(order => order.Id == id);

            if (order == null)
                return NotFound("Bestelling niet gevonden");

            order.IsDelivered = state;
            if (state)
            {
                order.TimeDelivered = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var order = await _context.Orders
                .Include(order => order.Reservation)
                .Include(order => order.ProductOrders)
                .ThenInclude(productOrder => productOrder.Product)
                .FirstOrDefaultAsync(order => order.Id == id);

            if (order == null)
                return NotFound("Bestelling niet gevonden");

            return Ok(order);
        }

        [HttpGet("GetProducts/{id}")]
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        public async Task<ActionResult<Dictionary<string, ProductOrder[]>>> GetProducts(int id)
        {
            var order = await _context.Orders
                .Include(order => order.Reservation)
                .Include(order => order.ProductOrders)
                    .ThenInclude(productOrder => productOrder.Product)
                        .ThenInclude(product => product.Category)
                .FirstOrDefaultAsync(order => order.Id == id);

            if (order == null)
                return NotFound("Bestelling niet gevonden");

            var dic = new Dictionary<string, ProductOrder[]>();
            var dicList = new Dictionary<string, List<ProductOrder>>();

            foreach (var productOrder in order.ProductOrders)
            {
                if (!dicList.ContainsKey(productOrder.Product.Category.Name))
                {
                    dicList.Add(productOrder.Product.Category.Name, new List<ProductOrder>());
                }
                dicList[productOrder.Product.Category.Name].Add(productOrder);
            }

            foreach (KeyValuePair<string, List<ProductOrder>> entry in dicList)
            {
                dic.Add(entry.Key, entry.Value.ToArray());
            }

            return Ok(dic);
        }
    }
}