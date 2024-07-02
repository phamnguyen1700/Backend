using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BE_V2.DataDB;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceDetailController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public PriceDetailController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // POST: api/PriceDetail
        [HttpPost]
        public async Task<ActionResult<PriceDetail>> CreatePriceDetail([FromBody] PriceDetailDTO priceDetailDto)
        {
            // Kiểm tra xem ProductID có tồn tại không
            var product = await _context.Products.FindAsync(priceDetailDto.ProductID);
            if (product == null)
            {
                return NotFound($"Product with ID {priceDetailDto.ProductID} not found.");
            }

            // Chuyển DTO thành entity
            var priceDetail = new PriceDetail
            {
                ProductID = priceDetailDto.ProductID,
                DiamondPrice = priceDetailDto.DiamondPrice,
                JewelryPrice = priceDetailDto.JewelryPrice,
                ProcessingPrice = priceDetailDto.ProcessingPrice,
                Profit = priceDetailDto.Profit
            };

            _context.PriceDetails.Add(priceDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPriceDetail), new { id = priceDetail.PriceDetailID }, priceDetail);
        }

        // GET: api/PriceDetail/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PriceDetail>> GetPriceDetail(int id)
        {
            var priceDetail = await _context.PriceDetails
                                            .Include(pd => pd.Product)
                                            .FirstOrDefaultAsync(pd => pd.PriceDetailID == id);

            if (priceDetail == null)
            {
                return NotFound();
            }

            return priceDetail;
        }
    }
}
