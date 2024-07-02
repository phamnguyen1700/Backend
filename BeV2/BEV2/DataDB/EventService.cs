using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;

public class EventService
{
    private readonly DiamondShopV4Context _context;

    public EventService(DiamondShopV4Context context)
    {
        _context = context;
    }

    public async Task CleanupExpiredEvents()
    {
        var expiredEvents = await _context.Events
            .Where(e => e.Date < DateTime.UtcNow) // Lọc các sự kiện hết hạn
            .Include(e => e.EventItems) // Bao gồm EventItems vì EventItem nằm bên trong Event
            .ToListAsync();

        foreach (var expiredEvent in expiredEvents)
        {
            foreach (var eventItem in expiredEvent.EventItems)
            {
                // Tìm sản phẩm tương ứng với mục sự kiện
                var product = await _context.Products.FindAsync(eventItem.ProductID);
                if (product != null)
                {
                    // Khôi phục lại giá gốc của sản phẩm
                    product.Price = product.Price / (1 - (eventItem.Discount / 100)); // Công thức trả lại giá ban đầu
                    _context.Entry(product).State = EntityState.Modified; // Đánh dấu thay đổi
                }

                // Xóa EventItem
                _context.EventItems.Remove(eventItem);
            }

            // Xóa Event
            _context.Events.Remove(expiredEvent);
        }

        await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
    }
}
