using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public CartController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<Cart>> CreateCart([FromBody] int userId)
        {
            var existingCart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (existingCart != null)
            {
                return Conflict("Cart already exists for this user.");
            }

            var cart = new Cart
            {
                UserID = userId,
                CreatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem>()
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCartByUserId), new { userId = cart.UserID }, cart);
        }

        // GET: api/Cart/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null)
            {
                return NotFound();
            }

            return cart.CartItems.ToList();
        }

        [HttpGet("User/{userId}/Count")]
        public async Task<ActionResult<int>> GetCartItemCount(int userId)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                                           .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null)
            {
                return NotFound(new { Message = "Cart not found for userId: " + userId });
            }

            return Ok(cart.CartItems.Count);
        }

        [HttpGet("User/{userId}")]
        public async Task<ActionResult<Cart>> GetCartByUserId(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }



        private bool CartExists(int userId)
        {
            return _context.Carts.Any(e => e.UserID == userId);
        }
    }
}
