using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
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
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                return NotFound("User not found.");
            }

            var existingCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserID == userId);

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

        // GET: api/Cart/User/{userId}
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

        // DELETE: api/Cart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.CartID == id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(int userId)
        {
            return _context.Carts.Any(e => e.UserID == userId);
        }
    }
}
