using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BE_V2.DataDB
{
    public class OrderService
    {
        private readonly DiamondShopV4Context _context;

        public OrderService(DiamondShopV4Context context)
        {
            _context = context;
        }

        public async Task CleanupOrdersAsync()
        {
            var now = DateTime.UtcNow;
            var ordersToDelete = await _context.Orders
                .Include(o => o.OrderLogs)
                .Where(o => o.OrderLogs.Any(log => log.Phase1 == false && EF.Functions.DateDiffMinute(log.TimePhase1, now) >= 2))
                .ToListAsync();

            if (ordersToDelete.Any())
            {
                foreach (var order in ordersToDelete)
                {
                    var orderLogs = _context.OrderLogs.Where(log => log.OrderID == order.OrderId);
                    _context.OrderLogs.RemoveRange(orderLogs);
                }

                _context.Orders.RemoveRange(ordersToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
