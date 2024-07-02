using BE_V2.DataDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RingMoldController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public RingMoldController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // GET: api/RingMold
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RingMold>>> GetRingMolds()
        {
            return await _context.RingMold.ToListAsync();
        }

        // GET: api/RingMold/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RingMold>> GetRingMold(int id)
        {
            var ringMold = await _context.RingMold.FindAsync(id);

            if (ringMold == null)
            {
                return NotFound();
            }

            return ringMold;
        }

        // PUT: api/RingMold/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRingMold(int id, RingMold ringMold)
        {
            if (id != ringMold.RingMoldId)
            {
                return BadRequest();
            }

            _context.Entry(ringMold).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RingMoldExists(id))
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

        // POST: api/RingMold
        [HttpPost]
        public async Task<ActionResult<RingMold>> PostRingMold(RingMold ringMold)
        {
            _context.RingMold.Add(ringMold);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRingMold), new { id = ringMold.RingMoldId }, ringMold);
        }

        // DELETE: api/RingMold/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRingMold(int id)
        {
            var ringMold = await _context.RingMold.FindAsync(id);
            if (ringMold == null)
            {
                return NotFound();
            }

            _context.RingMold.Remove(ringMold);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RingMoldExists(int id)
        {
            return _context.RingMold.Any(e => e.RingMoldId == id);
        }
    }
}
