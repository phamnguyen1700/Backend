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

    // GET: api/RingPriceTable/{material}/{caratWeight}/sizes
    [HttpGet("{material}/{caratWeight}/sizes")]
    public async Task<ActionResult<IEnumerable<decimal>>> GetRingSizes(string material, decimal caratWeight)
    {
        var sizes = await _context.RingPriceTable
            .Where(rpt => rpt.Material == material && rpt.CaratWeight == caratWeight)
            .Select(rpt => rpt.Size)
            .Distinct()
            .ToListAsync();

        return Ok(sizes);
    }
}
