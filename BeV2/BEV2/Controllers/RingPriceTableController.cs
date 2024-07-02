using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class RingPriceTableController : ControllerBase
{
    private readonly DiamondShopV4Context _context;

    public RingPriceTableController(DiamondShopV4Context context)
    {
        _context = context;
    }

    // GET: api/RingPriceTable
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RingPriceTable>>> GetRingPriceTables()
    {
        return await _context.RingPriceTable.ToListAsync();
    }

    // GET: api/RingPriceTable/{material}/{size}/{caratWeight}
    [HttpGet("{material}/{size}/{caratWeight}")]
    public async Task<ActionResult<RingPriceTable>> GetRingPriceTable(string material, decimal size, decimal caratWeight)
    {
        var ringPriceTable = await _context.RingPriceTable.FindAsync(material, size, caratWeight);

        if (ringPriceTable == null)
        {
            return NotFound();
        }

        return ringPriceTable;
    }
}
