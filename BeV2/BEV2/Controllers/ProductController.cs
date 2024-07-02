using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public ProductsController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.MainDiamond)
                .Include(p => p.SecondaryDiamond)
                .Include(p => p.ProductTypeNavigation)
                .Include(p => p.RingMold)
                .Include(p => p.NecklaceMold)
                .ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.MainDiamond)
                .Include(p => p.SecondaryDiamond)
                .Include(p => p.ProductTypeNavigation)
                .Include(p => p.RingMold)
                .Include(p => p.NecklaceMold)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Calculate and set the final price
            var finalPrice = CalculateFinalPrice(product);

            // Return product and final price
            return Ok(new
            {
                product = product,
                finalPrice = finalPrice
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            // Check if related entities exist
            if (product.ProductType != null && !_context.ProductTypes.Any(pt => pt.ProductTypeId == product.ProductType))
            {
                return BadRequest("Invalid ProductType.");
            }

            if (product.MainDiamondId != null && !_context.Diamonds.Any(d => d.DiamondId == product.MainDiamondId))
            {
                return BadRequest("Invalid MainDiamondId.");
            }

            if (product.SecondaryDiamondId != null && !_context.Diamonds.Any(d => d.DiamondId == product.SecondaryDiamondId))
            {
                return BadRequest("Invalid SecondaryDiamondId.");
            }

            if (product.ProductType == 2 && product.RingMoldId != null && !_context.RingMold.Any(rm => rm.RingMoldId == product.RingMoldId))
            {
                return BadRequest("Invalid RingMoldId.");
            }

            if (product.ProductType == 3 && product.NecklaceMoldId != null && !_context.NecklaceMold.Any(nm => nm.NecklaceMoldId == product.NecklaceMoldId))
            {
                return BadRequest("Invalid NecklaceMoldId.");
            }

            if (product.ProductType == 2)
            {
                product.NecklaceMoldId = null;
            }
            else if (product.ProductType == 3)
            {
                product.RingMoldId = null;
            }

            // Calculate and set the final price
            product.Price = CalculateFinalPrice(product);

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // Validate the ProductType
            if (product.ProductType == null || !_context.ProductTypes.Any(pt => pt.ProductTypeId == product.ProductType))
            {
                return BadRequest("Invalid ProductType.");
            }

            // Validate MainDiamondId
            if (product.MainDiamondId != null && !_context.Diamonds.Any(d => d.DiamondId == product.MainDiamondId))
            {
                return BadRequest("Invalid MainDiamondId.");
            }

            // Validate SecondaryDiamondId
            if (product.SecondaryDiamondId != null && !_context.Diamonds.Any(d => d.DiamondId == product.SecondaryDiamondId))
            {
                return BadRequest("Invalid SecondaryDiamondId.");
            }

            // Validate MoldId based on ProductType
            if (product.ProductType == 2)
            {
                if (product.RingMoldId == null || !_context.RingMold.Any(rm => rm.RingMoldId == product.RingMoldId))
                {
                    return BadRequest("Invalid RingMoldId for Ring.");
                }
                product.NecklaceMoldId = null;
            }
            else if (product.ProductType == 3)
            {
                if (product.NecklaceMoldId == null || !_context.NecklaceMold.Any(nm => nm.NecklaceMoldId == product.NecklaceMoldId))
                {
                    return BadRequest("Invalid NecklaceMoldId for Necklace.");
                }
                product.RingMoldId = null;
            }

            // Calculate and set the final price
            product.Price = CalculateFinalPrice(product);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        public decimal CalculateFinalPrice(Product product)
        {
            // Fetch prices
            decimal moldPrice = 0;
            decimal mainDiamondPrice = 0;
            decimal secondaryDiamondPrice = 0;

            if (product.ProductType == 2 && product.RingMoldId != null)
            {
                var ringMold = _context.RingMold.Find(product.RingMoldId);
                if (ringMold != null)
                {
                    moldPrice = ringMold.BasePrice;
                }
            }
            else if (product.ProductType == 3 && product.NecklaceMoldId != null)
            {
                var necklaceMold = _context.NecklaceMold.Find(product.NecklaceMoldId);
                if (necklaceMold != null)
                {
                    moldPrice = necklaceMold.BasePrice;
                }
            }

            if (product.MainDiamondId != null)
            {
                var mainDiamond = _context.Diamonds.Find(product.MainDiamondId);
                if (mainDiamond != null)
                {
                    var mainDiamondPriceEntry = _context.DiamondPriceTable
                        .FirstOrDefault(d => d.Carat == mainDiamond.CaratWeight && d.Color == mainDiamond.Color && d.Clarity == mainDiamond.Clarity && d.Cut == mainDiamond.Cut);
                    if (mainDiamondPriceEntry != null)
                    {
                        mainDiamondPrice = mainDiamondPriceEntry.Price;
                    }
                }
            }

            if (product.SecondaryDiamondId != null)
            {
                var secondaryDiamond = _context.Diamonds.Find(product.SecondaryDiamondId);
                if (secondaryDiamond != null)
                {
                    var secondaryDiamondPriceEntry = _context.DiamondPriceTable
                        .FirstOrDefault(d => d.Carat == secondaryDiamond.CaratWeight && d.Color == secondaryDiamond.Color && d.Clarity == secondaryDiamond.Clarity && d.Cut == secondaryDiamond.Cut);
                    if (secondaryDiamondPriceEntry != null)
                    {
                        secondaryDiamondPrice = secondaryDiamondPriceEntry.Price;
                    }
                }
            }

            // Calculate the final price
            decimal finalPrice = (moldPrice + mainDiamondPrice + (secondaryDiamondPrice * (product.SecondaryDiamondCount ?? 0)) + (product.ProcessingPrice ?? 0)) * (product.ExchangeRate ?? 1);
            return finalPrice;
        }
    }
}
