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
    public class DiamondsController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public DiamondsController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // GET: api/Diamonds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Diamond>>> GetDiamonds()
        {
            return await _context.Diamonds.ToListAsync();
        }

        // GET: api/Diamonds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Diamond>> GetDiamond(int id)
        {
            var diamond = await _context.Diamonds.FindAsync(id);

            if (diamond == null)
            {
                return NotFound();
            }

            return diamond;
        }

        // PUT: api/Diamonds/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiamond(int id, Diamond diamond)
        {
            if (id != diamond.DiamondId)
            {
                return BadRequest();
            }

            _context.Entry(diamond).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiamondExists(id))
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

        // POST: api/Diamonds
        [HttpPost]
        public async Task<ActionResult<Diamond>> PostDiamond(Diamond diamond)
        {
            _context.Entry(diamond).Property(d => d.DiamondId).IsTemporary = true;

            _context.Diamonds.Add(diamond);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiamond", new { id = diamond.DiamondId }, diamond);
        }

        // DELETE: api/Diamonds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiamond(int id)
        {
            var diamond = await _context.Diamonds.FindAsync(id);
            if (diamond == null)
            {
                return NotFound();
            }

            _context.Diamonds.Remove(diamond);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiamondExists(int id)
        {
            return _context.Diamonds.Any(e => e.DiamondId == id);
        }

        
    }
}
