using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using Newtonsoft.Json;

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
                        switch (mainDiamond.Origin)
                        {
                            case "South Africa":
                                mainDiamondPrice *= 1.1m; // Apply 10% increase for South Africa
                                break;
                            case "Russia":
                                mainDiamondPrice *= 1.2m; // Apply 20% increase for Russia
                                break;
                            case "Canada":
                                mainDiamondPrice *= 1.15m; // Apply 15% increase for Canada
                                break;
                            case "Botswana":
                                mainDiamondPrice *= 1.25m; // Apply 25% increase for Botswana
                                break;
                            default:
                                break;
                        }
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
                        switch (secondaryDiamond.Origin)
                        {
                            case "South Africa":
                                secondaryDiamondPrice *= 1.1m; // Apply 10% increase for South Africa
                                break;
                            case "Russia":
                                secondaryDiamondPrice *= 1.2m; // Apply 20% increase for Russia
                                break;
                            case "Canada":
                                secondaryDiamondPrice *= 1.15m; // Apply 15% increase for Canada
                                break;
                            case "Botswana":
                                secondaryDiamondPrice *= 1.25m; // Apply 25% increase for Botswana
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            // Calculate the final price
            decimal exchangeRateMultiplier = (product.ExchangeRate ?? 0) / 100 + 1;
            decimal finalPrice = (moldPrice + mainDiamondPrice + (secondaryDiamondPrice * (product.SecondaryDiamondCount ?? 0)) + (product.ProcessingPrice ?? 0)) * exchangeRateMultiplier;

            finalPrice = Math.Floor(finalPrice / 100000) * 100000;
            return finalPrice;
        }


        [HttpPost("CreateOrGetProductWithSize")]
        public async Task<ActionResult<Product>> CreateOrGetProductWithSize([FromBody] CreateOrGetProductWithSizeRequest request)
        {
            // Fetch the existing product
            var existingProduct = await _context.Products
                .Include(p => p.MainDiamond)
                .Include(p => p.SecondaryDiamond)
                .Include(p => p.ProductTypeNavigation)
                .Include(p => p.RingMold)
                .Include(p => p.NecklaceMold)
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId);

            if (existingProduct == null)
            {
                return NotFound();
            }

            // Convert size to decimal for price table lookup
            if (!decimal.TryParse(request.Size, out decimal sizeDecimal))
            {
                return BadRequest("Invalid size format.");
            }

            // Fetch the ring price based on the selected size
            var ringPriceTable = await _context.RingPriceTable
                .FirstOrDefaultAsync(rpt => rpt.Material == existingProduct.RingMold.Material &&
                                            rpt.Size == sizeDecimal &&
                                            rpt.CaratWeight == existingProduct.RingMold.CaratWeight);

            if (ringPriceTable == null)
            {
                return NotFound("Ring price for the selected size not found.");
            }

            // Check if a ring mold with the requested size already exists
            var existingRingMold = await _context.RingMold
                .FirstOrDefaultAsync(rm => rm.Material == existingProduct.RingMold.Material &&
                                            rm.CaratWeight == existingProduct.RingMold.CaratWeight &&
                                            rm.Gender == existingProduct.RingMold.Gender &&
                                            rm.RingType == existingProduct.RingMold.RingType &&
                                            rm.Size == request.Size);

            if (existingRingMold != null)
            {
                // Return existing product with the requested ring mold size
                var productWithExistingSize = await _context.Products
                    .FirstOrDefaultAsync(p => p.RingMoldId == existingRingMold.RingMoldId && p.ProductType == 2);

                return Ok(productWithExistingSize);
            }

            // Create a new ring mold with the requested size
            var newRingMold = new RingMold
            {
                Material = existingProduct.RingMold.Material,
                CaratWeight = existingProduct.RingMold.CaratWeight,
                Gender = existingProduct.RingMold.Gender,
                RingType = existingProduct.RingMold.RingType,
                BasePrice = ringPriceTable.BasePrice, // Use the dynamically fetched price
                Size = request.Size
            };

            _context.RingMold.Add(newRingMold);
            await _context.SaveChangesAsync();

            // Create a new product with the new ring mold
            var newProduct = new Product
            {
                ProductName = existingProduct.ProductName,
                ProductType = existingProduct.ProductType,
                Material = existingProduct.Material,
                Size = request.Size,
                Description = existingProduct.Description,
                Price = CalculateNewPrice(existingProduct, newRingMold.BasePrice),
                ProcessingPrice = existingProduct.ProcessingPrice,
                ExchangeRate = existingProduct.ExchangeRate,
                Quantity = existingProduct.Quantity,
                MainDiamondId = existingProduct.MainDiamondId,
                Image1 = existingProduct.Image1,
                Image2 = existingProduct.Image2,
                Image3 = existingProduct.Image3,
                SecondaryDiamondId = existingProduct.SecondaryDiamondId,
                RingMoldId = newRingMold.RingMoldId
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = newProduct.ProductId }, newProduct);
        }

        public class CreateOrGetProductWithSizeRequest
        {
            public int ProductId { get; set; }
            public string Size { get; set; }
        }

        private decimal CalculateNewPrice(Product product, decimal newRingMoldBasePrice)
        {
            // Fetch prices
            decimal mainDiamondPrice = 0;
            decimal secondaryDiamondPrice = 0;

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
                        switch (mainDiamond.Origin)
                        {
                            case "South Africa":
                                mainDiamondPrice *= 1.1m; // Apply 10% increase for South Africa
                                break;
                            case "Russia":
                                mainDiamondPrice *= 1.2m; // Apply 20% increase for Russia
                                break;
                            case "Canada":
                                mainDiamondPrice *= 1.15m; // Apply 15% increase for Canada
                                break;
                            case "Botswana":
                                mainDiamondPrice *= 1.25m; // Apply 25% increase for Botswana
                                break;
                            default:
                                break;
                        }
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
                        switch (secondaryDiamond.Origin)
                        {
                            case "South Africa":
                                secondaryDiamondPrice *= 1.1m; // Apply 10% increase for South Africa
                                break;
                            case "Russia":
                                secondaryDiamondPrice *= 1.2m; // Apply 20% increase for Russia
                                break;
                            case "Canada":
                                secondaryDiamondPrice *= 1.15m; // Apply 15% increase for Canada
                                break;
                            case "Botswana":
                                secondaryDiamondPrice *= 1.25m; // Apply 25% increase for Botswana
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            // Calculate the final price
            decimal exchangeRateMultiplier = (product.ExchangeRate ?? 0) / 100 + 1;
            decimal finalPrice = (newRingMoldBasePrice + mainDiamondPrice + (secondaryDiamondPrice * (product.SecondaryDiamondCount ?? 0)) + (product.ProcessingPrice ?? 0)) * exchangeRateMultiplier;

            finalPrice = Math.Floor(finalPrice / 100000) * 100000;
            return finalPrice;
        }


        [HttpPost("CalculatePrice")]
        public async Task<ActionResult<decimal>> CalculatePrice(CalculatePriceRequest request)
        {
            decimal moldPrice = 0;
            decimal mainDiamondPrice = 0;
            decimal secondaryDiamondPrice = 0;

            // Fetch the ring mold price based on the size
            if (request.ProductType == 2)
            {
                var ringPriceTable = await _context.RingPriceTable
                    .FirstOrDefaultAsync(rpt => rpt.Material == request.Material &&
                                                rpt.Size == decimal.Parse(request.Size) &&
                                                rpt.CaratWeight == request.CaratWeight);
                if (ringPriceTable != null)
                {
                    moldPrice = ringPriceTable.BasePrice;
                }
            }

            // Fetch the main diamond price
            if (request.MainDiamondId != null)
            {
                var mainDiamond = _context.Diamonds.Find(request.MainDiamondId);
                if (mainDiamond != null)
                {
                    var mainDiamondPriceEntry = _context.DiamondPriceTable
                        .FirstOrDefault(d => d.Carat == mainDiamond.CaratWeight && d.Color == mainDiamond.Color && d.Clarity == mainDiamond.Clarity && d.Cut == mainDiamond.Cut);
                    if (mainDiamondPriceEntry != null)
                    {
                        mainDiamondPrice = mainDiamondPriceEntry.Price;
                        switch (mainDiamond.Origin)
                        {
                            case "South Africa":
                                mainDiamondPrice *= 1.1m; // Apply 10% increase for South Africa
                                break;
                            case "Russia":
                                mainDiamondPrice *= 1.2m; // Apply 20% increase for Russia
                                break;
                            case "Canada":
                                mainDiamondPrice *= 1.15m; // Apply 15% increase for Canada
                                break;
                            case "Botswana":
                                mainDiamondPrice *= 1.25m; // Apply 25% increase for Botswana
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (request.SecondaryDiamondId != null)
            {
                var secondaryDiamond = _context.Diamonds.Find(request.SecondaryDiamondId);
                if (secondaryDiamond != null)
                {
                    var secondaryDiamondPriceEntry = _context.DiamondPriceTable
                        .FirstOrDefault(d => d.Carat == secondaryDiamond.CaratWeight && d.Color == secondaryDiamond.Color && d.Clarity == secondaryDiamond.Clarity && d.Cut == secondaryDiamond.Cut);
                    if (secondaryDiamondPriceEntry != null)
                    {
                        secondaryDiamondPrice = secondaryDiamondPriceEntry.Price;
                        switch (secondaryDiamond.Origin)
                        {
                            case "South Africa":
                                secondaryDiamondPrice *= 1.1m; // Apply 10% increase for South Africa
                                break;
                            case "Russia":
                                secondaryDiamondPrice *= 1.2m; // Apply 20% increase for Russia
                                break;
                            case "Canada":
                                secondaryDiamondPrice *= 1.15m; // Apply 15% increase for Canada
                                break;
                            case "Botswana":
                                secondaryDiamondPrice *= 1.25m; // Apply 25% increase for Botswana
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            // Calculate the final price
            decimal exchangeRateMultiplier = (request.ExchangeRate ?? 0) / 100 + 1;
            decimal finalPrice = (moldPrice + mainDiamondPrice + (secondaryDiamondPrice * (request.SecondaryDiamondCount ?? 0)) + (request.ProcessingPrice ?? 0)) * exchangeRateMultiplier;

            finalPrice = Math.Floor(finalPrice / 100000) * 100000;
            return Ok(finalPrice);
        }

        public class CalculatePriceRequest
        {
            public int ProductType { get; set; }
            public string Material { get; set; }
            public string Size { get; set; }
            public decimal CaratWeight { get; set; }
            public int? MainDiamondId { get; set; }
            public int? SecondaryDiamondId { get; set; }
            public int? SecondaryDiamondCount { get; set; }
            public decimal? ProcessingPrice { get; set; }
            public decimal? ExchangeRate { get; set; }
        }



    }
}
