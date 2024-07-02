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
    public class WishlistItemsController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public WishlistItemsController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // POST: api/WishlistItems
        [HttpPost]
        public async Task<ActionResult<WishlistItem>> AddWishlistItem([FromBody] WishlistItemDTO wishlistItemDTO)
        {
            // Kiểm tra sự tồn tại của CustomerId trong bảng Customer
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == wishlistItemDTO.CustomerId);
            if (!customerExists)
            {
                return NotFound("Customer not found.");
            }

            // Tìm kiếm wishlist theo CustomerId
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.CustomerId == wishlistItemDTO.CustomerId);

            // Nếu không tồn tại, tạo một wishlist mới
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    CustomerId = wishlistItemDTO.CustomerId,
                    CreatedDate = DateTime.UtcNow,
                    WishlistItems = new List<WishlistItem>()
                };

                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra xem sản phẩm đã tồn tại trong wishlist chưa
            var existingWishlistItem = wishlist.WishlistItems
                .FirstOrDefault(wi => wi.ProductId == wishlistItemDTO.ProductId);

            if (existingWishlistItem != null)
            {
                return Conflict("Product already exists in the wishlist.");
            }

            // Thêm sản phẩm mới vào wishlist
            var wishlistItem = new WishlistItem
            {
                WishlistId = wishlist.WishlistId,
                ProductId = wishlistItemDTO.ProductId,
                AddedDate = wishlistItemDTO.AddedDate
            };

            wishlist.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWishlistItem), new { id = wishlistItem.WishlistItemId }, wishlistItem);
        }

        // GET: api/WishlistItems/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<WishlistItem>> GetWishlistItem(int id)
        {
            var wishlistItem = await _context.WishlistItems
                .Include(wi => wi.Product)
                .FirstOrDefaultAsync(wi => wi.WishlistItemId == id);

            if (wishlistItem == null)
            {
                return NotFound();
            }

            return wishlistItem;
        }

        // DELETE: api/WishlistItems/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWishlistItem(int id)
        {
            var wishlistItem = await _context.WishlistItems.FindAsync(id);
            if (wishlistItem == null)
            {
                return NotFound();
            }

            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }




        private bool WishlistItemExists(int id)
        {
            return _context.WishlistItems.Any(e => e.WishlistItemId == id);
        }
    }
}
