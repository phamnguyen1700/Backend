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
    public class EventItemsController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public EventItemsController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // GET: api/EventItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventItem>>> GetEventItems()
        {
            return await _context.EventItems.ToListAsync();
        }

        // GET: api/EventItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventItem>> GetEventItem(int id)
        {
            var eventItem = await _context.EventItems.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return eventItem;
        }

        // PUT: api/EventItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventItem(int id, [FromBody] EventItemDTO eventItemDTO)
        {
            if (id != eventItemDTO.EventItemID)
            {
                return BadRequest();
            }
            var eventItem = await _context.EventItems.FindAsync(id);//tim kiem
            if (eventItem == null)
            {
                return NotFound();
            }
            eventItem.Date = eventItemDTO.Date;//cap nhat lai thuoc tinh voi cac gia tri tu eventItemDTO
            eventItem.Discount = eventItemDTO.Discount;
            _context.Entry(eventItem).State = EntityState.Modified;
            var product = await _context.Products.FindAsync(eventItem.ProductID);
            if (product != null)
            {
                //Khoi phuc lai gia goc truoc khi ap dung giam gia moi
                product.Price = product.Price / (1 - (eventItem.Discount / 100));
                product.Price = product.Price - (product.Price * eventItem.Discount / 100);
                _context.Entry(product).State = EntityState.Modified;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventItemExists(id))
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

        // POST: api/EventItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EventItem>> AddEventItem([FromBody] EventItemDTO eventItemDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var eventExists = await _context.Events.AnyAsync(e => e.EventID == eventItemDTO.EventID);
            if (!eventExists)
            {
                return BadRequest("Event not found");
            }
            var productExsist = await _context.Products.AnyAsync(p => p.ProductId == eventItemDTO.ProductID);
            if (!productExsist)
            {
                return BadRequest("Product not found");
            }
            var eventItem = new EventItem
            {
                EventID = eventItemDTO.EventID,
                ProductID = eventItemDTO.ProductID,
                Date = eventItemDTO.Date,
                Discount = eventItemDTO.Discount,
            };
            _context.EventItems.Add(eventItem);
            await _context.SaveChangesAsync();
            var product = await _context.Products.FindAsync(eventItemDTO.ProductID);
            if (product != null)
            {
                //gia san pham sau khi giam 
                product.Price = product.Price - (product.Price * eventItem.Discount / 100);
                _context.Entry(product).State = EntityState.Modified; //danh dau thay doi
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetEventItem), new { id = eventItem.EventItemID }, eventItem);

        }

        // DELETE: api/EventItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventItem(int id)
        {
            var eventItem = await _context.EventItems.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(eventItem.ProductID);
            if (product != null)
            {
                //khoi phuc gia
                product.Price = product.Price / (1 - (eventItem.Discount / 100));
                _context.Entry(product).State = EntityState.Modified;
            }
            _context.EventItems.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventItemExists(int id)
        {
            return _context.EventItems.Any(e => e.EventItemID == id);
        }
    }
}
