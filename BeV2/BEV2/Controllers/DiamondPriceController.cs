using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using System.Linq;
using System.Threading.Tasks;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondPriceController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public DiamondPriceController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // GET: api/DiamondPrice?carat=0.2&color=E&clarity=VVS1&cut=Ideal
        [HttpGet]
        public async Task<ActionResult<DiamondPriceTable>> GetDiamondPrice(decimal carat, string color, string clarity, string cut)
        {
            var diamondPrice = await _context.DiamondPriceTable
                .Where(d => d.Carat == carat && d.Color == color && d.Clarity == clarity && d.Cut == cut)
                .FirstOrDefaultAsync();

            if (diamondPrice == null)
            {
                return NotFound();
            }

            return diamondPrice;
        }
    }
}
