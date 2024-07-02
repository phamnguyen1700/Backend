using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using System.Linq;
using System.Threading.Tasks;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public CartItemController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // POST: api/CartItem
        [HttpPost]
        public async Task<ActionResult<CartItem>> AddCartItem([FromBody] CartItemDTO cartItemDTO)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.CartID == cartItemDTO.CartID);

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductID == cartItemDTO.ProductID && ci.Size == cartItemDTO.Size);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartItemDTO.Quantity;
                _context.Entry(existingCartItem).State = EntityState.Modified;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartID = cartItemDTO.CartID,
                    ProductID = cartItemDTO.ProductID,
                    Quantity = cartItemDTO.Quantity,
                    Price = cartItemDTO.Price,
                    Size = cartItemDTO.Size // Include the Size property
                };
                cart.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCartItem), new { id = cartItemDTO.CartID }, cartItemDTO);
        }

        // GET: api/CartItem/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            var cartItem = await _context.CartItems.Include(ci => ci.Product).FirstOrDefaultAsync(ci => ci.CartItemID == id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return cartItem;
        }

        // PUT: api/CartItem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, CartItem cartItem)
        {
            if (id != cartItem.CartItemID)
            {
                return BadRequest();
            }

            _context.Entry(cartItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/CartItem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.CartItemID == id);
        }
    }
}
