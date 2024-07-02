using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using System.Linq;
using System.Threading.Tasks;


[Route("api/[controller]")]
[ApiController]
public class NecklacePriceTableController : ControllerBase
{
    private readonly DiamondShopV4Context _context;

    public NecklacePriceTableController(DiamondShopV4Context context)
    {
        _context = context;
    }

    // GET: api/NecklacePriceTable
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NecklacePriceTable>>> GetNecklacePriceTables()
    {
        return await _context.NecklacePriceTable.ToListAsync();
    }

    // GET: api/NecklacePriceTable/{material}/{length}/{caratWeight}
    [HttpGet("{material}/{length}/{caratWeight}")]
    public async Task<ActionResult<NecklacePriceTable>> GetNecklacePriceTable(string material, int length, decimal caratWeight)
    {
        var necklacePriceTable = await _context.NecklacePriceTable.FindAsync(material, length, caratWeight);

        if (necklacePriceTable == null)
        {
            return NotFound();
        }

        return necklacePriceTable;
    }
}
