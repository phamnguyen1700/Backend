using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondPriceController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;
        private readonly ILogger<DiamondPriceController> _logger;

        public DiamondPriceController(DiamondShopV4Context context, ILogger<DiamondPriceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/DiamondPrice?carat=0.2&color=E&clarity=VVS1&cut=Ideal
        [HttpGet]
        public async Task<ActionResult<decimal>> GetDiamondPrice(decimal carat, string color, string clarity, string cut)
        {
            var diamondPrice = await _context.DiamondPriceTable
                .Where(d => d.Carat == carat && d.Color == color && d.Clarity == clarity && d.Cut == cut)
                .FirstOrDefaultAsync();

            if (diamondPrice == null)
            {
                return NotFound();
            }

            var diamond = await _context.Diamonds
                .Where(d => d.CaratWeight == carat && d.Color == color && d.Clarity == clarity && d.Cut == cut)
                .FirstOrDefaultAsync();

            if (diamond == null)
            {
                return NotFound();
            }

            decimal finalPrice = diamondPrice.Price;

            Console.WriteLine($"Original price: {finalPrice}");

            // Adjust price based on origin
            switch (diamond.Origin)
            {
                case "South Africa":
                    finalPrice *= 1.1m; // Apply 10% increase for South Africa
                    break;
                case "Russia":
                    finalPrice *= 1.2m; // Apply 20% increase for Russia
                    break;
                case "Canada":
                    finalPrice *= 1.15m; // Apply 15% increase for Canada
                    break;
                case "Botswana":
                    finalPrice *= 1.25m; // Apply 25% increase for Botswana
                    break;
                default:
                    break;
            }

            Console.WriteLine($"Adjusted price for origin {diamond.Origin}: {finalPrice}");

            return finalPrice;
        }

        // GET: api/DiamondPrice/Attributes
        [HttpGet("Attributes")]
        public async Task<ActionResult<DiamondAttributes>> GetDiamondAttributes()
        {
            var carats = await _context.DiamondPriceTable.Select(d => d.Carat).Distinct().ToListAsync();
            var colors = await _context.DiamondPriceTable.Select(d => d.Color).Distinct().ToListAsync();
            var clarities = await _context.DiamondPriceTable.Select(d => d.Clarity).Distinct().ToListAsync();
            var cuts = await _context.DiamondPriceTable.Select(d => d.Cut).Distinct().ToListAsync();

            var attributes = new DiamondAttributes
            {
                Carats = carats,
                Colors = colors,
                Clarities = clarities,
                Cuts = cuts
            };

            return attributes;
        }
    }

    public class DiamondAttributes
    {
        public List<decimal> Carats { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Clarities { get; set; }
        public List<string> Cuts { get; set; }
    }
}
