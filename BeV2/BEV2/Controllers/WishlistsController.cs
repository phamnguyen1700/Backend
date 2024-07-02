using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using BE_V2.DataDB;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistsController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public WishlistsController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // POST: api/Wishlists
        [HttpPost]
        public async Task<ActionResult<Wishlist>> CreateWishlist([FromBody] int customerId)
        {
            // Kiểm tra sự tồn tại của CustomerId trong bảng Customer
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
            if (!customerExists)
            {
                return NotFound("Customer not found.");
            }

            // Tìm kiếm một wishlist hiện có của người dùng dựa trên customerId
            var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.CustomerId == customerId);
            // Nếu đã có wishlist cho người dùng này, trả về mã trạng thái 409 Conflict
            if (wishlist != null)
            {
                return Conflict("Wishlist already exists for this customer.");
            }
            // Nếu không có wishlist nào cho người dùng này, tạo một wishlist mới
            wishlist = new Wishlist
            {
                CustomerId = customerId,
                CreatedDate = DateTime.UtcNow,
                WishlistItems = new List<WishlistItem>()
            };
            // Thêm wishlist mới vào context và lưu thay đổi vào cơ sở dữ liệu
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWishlistByCustomerId), new { customerId = wishlist.CustomerId }, wishlist);
        }

        // GET: api/Wishlists/Customer/{customerId}
        [HttpGet("Customer/{customerId}")]
        public async Task<ActionResult<Wishlist>> GetWishlistByCustomerId(int customerId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.CustomerId == customerId);

            if (wishlist == null)
            {
                return NotFound();
            }

            return wishlist;
        }

        // DELETE: api/Wishlists/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWishlist(int id)
        {
            var wishlist = await _context.Wishlists.Include(w => w.WishlistItems).FirstOrDefaultAsync(w => w.WishlistId == id);
            if (wishlist == null)
            {
                return NotFound();
            }

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Customer/{customerId}/clear")]
        public async Task<IActionResult> ClearWishlist(int customerId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.CustomerId == customerId);

            if (wishlist == null)
            {
                return NotFound("Wishlist not found.");
            }

            _context.WishlistItems.RemoveRange(wishlist.WishlistItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
