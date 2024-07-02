using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public EventsController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var @event = await _context.Events.Include(e => e.EventItems)
                                              .ThenInclude(ei => ei.Product)
                                              .FirstOrDefaultAsync(e => e.EventID == id);

            if (@event == null)
            {
                return NotFound();
            }

            return @event;
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDTO eventDTO)
        {
            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.EventName = eventDTO.EventName;
            existingEvent.Date = eventDTO.Date;
            existingEvent.Description = eventDTO.Description;

            _context.Entry(existingEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
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

        // POST: api/Events
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(EventDTO eventDTO)
        {
            var @event = new Event
            {
                EventName = eventDTO.EventName,
                Date = eventDTO.Date,
                Description = eventDTO.Description
            };

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = @event.EventID }, @event);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var @event = await _context.Events.Include(e => e.EventItems)
                                              .FirstOrDefaultAsync(e => e.EventID == id);

            if (@event == null)
            {
                return NotFound();
            }

            foreach (var eventItem in @event.EventItems)
            {
                var product = await _context.Products.FindAsync(eventItem.ProductID);
                if (product != null)
                {
                    // Tính toán lại giá sản phẩm
                    product.Price = product.Price / (1 - (eventItem.Discount / 100));
                }
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}
