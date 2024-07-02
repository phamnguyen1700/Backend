using BE_V2.DataDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NecklaceMoldController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public NecklaceMoldController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // GET: api/NecklaceMold
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NecklaceMold>>> GetNecklaceMolds()
        {
            return await _context.NecklaceMold.ToListAsync();
        }

        // GET: api/NecklaceMold/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NecklaceMold>> GetNecklaceMold(int id)
        {
            var necklaceMold = await _context.NecklaceMold.FindAsync(id);

            if (necklaceMold == null)
            {
                return NotFound();
            }

            return necklaceMold;
        }

        // PUT: api/NecklaceMold/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNecklaceMold(int id, NecklaceMold necklaceMold)
        {
            if (id != necklaceMold.NecklaceMoldId)
            {
                return BadRequest();
            }

            _context.Entry(necklaceMold).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NecklaceMoldExists(id))
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

        // POST: api/NecklaceMold
        [HttpPost]
        public async Task<ActionResult<NecklaceMold>> PostNecklaceMold(NecklaceMold necklaceMold)
        {
            _context.NecklaceMold.Add(necklaceMold);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNecklaceMold), new { id = necklaceMold.NecklaceMoldId }, necklaceMold);
        }

        // DELETE: api/NecklaceMold/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNecklaceMold(int id)
        {
            var necklaceMold = await _context.NecklaceMold.FindAsync(id);
            if (necklaceMold == null)
            {
                return NotFound();
            }

            _context.NecklaceMold.Remove(necklaceMold);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NecklaceMoldExists(int id)
        {
            return _context.NecklaceMold.Any(e => e.NecklaceMoldId == id);
        }
    }

}
