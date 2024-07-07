using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public OrdersController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderDTO orderDTO)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == orderDTO.UserID);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            var order = new Order
            {
                CustomerId = customer.CustomerId,
                TotalPrice = orderDTO.TotalPrice,
                OrderDate = DateOnly.FromDateTime(orderDTO.OrderDate)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderDetails = orderDTO.OrderDetails.Select(detail => new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = detail.ProductId,
                ProductName = detail.ProductName,
                ProductPrice = detail.ProductPrice,
                Quantity = detail.Quantity
            }).ToList();

            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();

            // Create an OrderLog entry
            var orderLog = new OrderLog
            {
                OrderID = order.OrderId,
                Phase1 = false,
                TimePhase1 = DateTime.UtcNow
            };
            _context.OrderLogs.Add(orderLog);
            await _context.SaveChangesAsync();

            // Clear the cart after successful order creation
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserID == orderDTO.UserID);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, new
            {
                order.OrderId,
                order.CustomerId,
                order.TotalPrice,
                order.OrderDate,
                OrderDetails = orderDetails
            });
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var result = new
            {
                order.OrderId,
                order.CustomerId,
                order.TotalPrice,
                order.OrderDate,
                OrderDetails = order.OrderDetails.Select(od => new
                {
                    od.OrderDetailId,
                    od.ProductId,
                    od.ProductName,
                    od.ProductPrice,
                    od.Quantity
                })
            };

            return Ok(result);
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
        }
    }
}
