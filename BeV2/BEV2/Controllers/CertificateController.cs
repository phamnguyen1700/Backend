using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public CertificateController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // GET: api/Certificate
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificates()
        {
            return await _context.Certificates.Include(c => c.Diamond).ToListAsync();
        }

        // GET: api/Certificate/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Certificate>> GetCertificate(int id)
        {
            var certificate = await _context.Certificates.Include(c => c.Diamond).FirstOrDefaultAsync(c => c.CertificateId == id);

            if (certificate == null)
            {
                return NotFound();
            }

            return certificate;
        }

        // POST: api/Certificate
        [HttpPost]
        public async Task<ActionResult<Certificate>> CreateCertificate(Certificate certificate)
        {
            // Ensure the DiamondId exists in the Diamond table
            var diamond = await _context.Diamonds.FindAsync(certificate.DiamondId);
            if (diamond == null)
            {
                return BadRequest(new { message = "Invalid DiamondId" });
            }

            certificate.Diamond = diamond;

            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCertificate), new { id = certificate.CertificateId }, certificate);
        }

        // PUT: api/Certificate/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCertificate(int id, Certificate certificate)
        {
            if (id != certificate.CertificateId)
            {
                return BadRequest();
            }

            _context.Entry(certificate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificateExists(id))
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

        // DELETE: api/Certificate/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CertificateExists(int id)
        {
            return _context.Certificates.Any(e => e.CertificateId == id);
        }
    }
}
