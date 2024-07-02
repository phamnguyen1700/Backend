using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using BE_V2.DTOs;

[Route("api/[controller]")]
[ApiController]
public class OrderLogsController : ControllerBase
{
    private readonly DiamondShopV4Context _context;

    public OrderLogsController(DiamondShopV4Context context)
    {
        _context = context;
    }

    // GET: api/OrderLogs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderLog>>> GetOrderLogs()
    {
        return await _context.OrderLogs.ToListAsync();
    }

    // GET: api/OrderLogs/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderLog>> GetOrderLog(int id)
    {
        var orderLog = await _context.OrderLogs.FindAsync(id);

        if (orderLog == null)
        {
            return NotFound();
        }

        return orderLog;
    }

    // GET: api/OrderLogs/order/5
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<OrderLog>> GetOrderLogByOrderId(int orderId)
    {
        var orderLog = await _context.OrderLogs.FirstOrDefaultAsync(ol => ol.OrderID == orderId);

        if (orderLog == null)
        {
            return NotFound();
        }

        return orderLog;
    }

    // POST: api/OrderLogs
    [HttpPost]
    public async Task<ActionResult<OrderLog>> PostOrderLog(OrderLogDTO orderLogDto)
    {
        // Check if the OrderID exists in the Orders table
        var orderExists = await _context.Orders.AnyAsync(o => o.OrderId == orderLogDto.OrderID);
        if (!orderExists)
        {
            return BadRequest("Invalid OrderID");
        }

        var orderLog = new OrderLog
        {
            OrderID = orderLogDto.OrderID,
            Phase1 = orderLogDto.Phase1,
            Phase2 = orderLogDto.Phase2,
            Phase3 = orderLogDto.Phase3,
            Phase4 = orderLogDto.Phase4,
            TimePhase1 = orderLogDto.TimePhase1 ?? DateTime.UtcNow,
            TimePhase2 = orderLogDto.TimePhase2 ?? DateTime.UtcNow,
            TimePhase3 = orderLogDto.TimePhase3 ?? DateTime.UtcNow,
            TimePhase4 = orderLogDto.TimePhase4 ?? DateTime.UtcNow
        };

        _context.OrderLogs.Add(orderLog);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrderLog), new { id = orderLog.LogID }, orderLog);
    }

    // PUT: api/OrderLogs/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatusUpdateDto updateDto)
    {
        var orderLog = await _context.OrderLogs.FindAsync(id);
        if (orderLog == null)
        {
            Console.WriteLine($"OrderLog with ID: {id} not found.");
            return NotFound();
        }

        Console.WriteLine($"Updating OrderLog with ID: {id}");
        Console.WriteLine($"Received StatusType: {updateDto.StatusType}");

        switch (updateDto.StatusType)
        {
            case "phase1":
                if (!orderLog.Phase1)
                {
                    orderLog.Phase1 = true;
                    orderLog.TimePhase1 = DateTime.UtcNow;
                    Console.WriteLine("Updated Phase1 and TimePhase1.");
                }
                break;
            case "phase2":
                if (!orderLog.Phase2)
                {
                    orderLog.Phase2 = true;
                    orderLog.TimePhase2 = DateTime.UtcNow;
                    Console.WriteLine("Updated Phase2 and TimePhase2.");
                }
                break;
            case "phase3":
                if (!orderLog.Phase3)
                {
                    orderLog.Phase3 = true;
                    orderLog.TimePhase3 = DateTime.UtcNow;
                    Console.WriteLine("Updated Phase3 and TimePhase3.");
                }
                break;
            case "phase4":
                if (!orderLog.Phase4)
                {
                    orderLog.Phase4 = true;
                    orderLog.TimePhase4 = DateTime.UtcNow;
                    Console.WriteLine("Updated Phase4 and TimePhase4.");
                }
                break;
            default:
                Console.WriteLine("Invalid status type received.");
                return BadRequest("Invalid status type");
        }

        _context.Entry(orderLog).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        Console.WriteLine($"OrderLog with ID: {id} successfully updated.");
        return NoContent();
    }
}
