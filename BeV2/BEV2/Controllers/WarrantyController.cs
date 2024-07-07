using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_V2.DataDB;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantiesController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public WarrantiesController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // POST: api/Warranties
        [HttpPost]
        public async Task<ActionResult<Warranty>> PostWarranty(WarrantyDTO warrantyDTO)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderId == warrantyDTO.OrderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var warranty = new Warranty
            {
                OrderId = order.OrderId,
                PurchaseDate = DateTime.UtcNow,
                WarrantyEndDate = warrantyDTO.WarrantyEndDate,
                StoreRepresentativeSignature = warrantyDTO.StoreRepresentativeSignature
            };

            _context.Warranties.Add(warranty);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWarranty), new { id = warranty.WarrantyId }, warranty);
        }

        // GET: api/Warranties/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetWarranty(int id)
        {
            var warranty = await _context.Warranties
                .Include(w => w.Order)
                    .ThenInclude(o => o.Customer)
                        .ThenInclude(c => c.User)
                .Include(w => w.Order)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                            .ThenInclude(p => p.MainDiamond)
                                .ThenInclude(d => d.Certificate)
                .FirstOrDefaultAsync(w => w.WarrantyId == id);

            if (warranty == null)
            {
                return NotFound();
            }

            var result = new
            {
                warranty.WarrantyId,
                warranty.PurchaseDate,
                warranty.WarrantyEndDate,
                warranty.StoreRepresentativeSignature,
                Order = new
                {
                    warranty.Order.OrderId,
                    Customer = new
                    {
                        warranty.Order.Customer.User.Name,
                        warranty.Order.Customer.User.PhoneNumber,
                        warranty.Order.Customer.User.Email,
                        warranty.Order.Customer.User.Address
                    },
                    OrderDetails = warranty.Order.OrderDetails.Select(od => new
                    {
                        od.ProductId,
                        od.ProductName,
                        od.Product.MainDiamond.Shape,
                        od.Product.MainDiamond.Cut,
                        od.Product.MainDiamond.Color,
                        od.Product.MainDiamond.Clarity,
                        od.Product.MainDiamond.CaratWeight,
                        CertificateId = od.Product.MainDiamond.Certificate.CertificateId
                    }).ToList()
                }
            };

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetWarranties()
        {
            var warranties = await _context.Warranties
                .Include(w => w.Order)
                    .ThenInclude(o => o.Customer)
                        .ThenInclude(c => c.User)
                .Include(w => w.Order)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                            .ThenInclude(p => p.MainDiamond)
                                .ThenInclude(d => d.Certificate)
                .ToListAsync();

            var result = warranties.Select(w => new
            {
                w.WarrantyId,
                w.PurchaseDate,
                w.WarrantyEndDate,
                w.StoreRepresentativeSignature,
                Order = new
                {
                    w.Order.OrderId,
                    Customer = new
                    {
                        w.Order.Customer.User.Name,
                        w.Order.Customer.User.PhoneNumber,
                        w.Order.Customer.User.Email,
                        w.Order.Customer.User.Address
                    },
                    OrderDetails = w.Order.OrderDetails.Select(od => new
                    {
                        od.ProductId,
                        od.ProductName,
                        od.Product.MainDiamond.Shape,
                        od.Product.MainDiamond.Cut,
                        od.Product.MainDiamond.Color,
                        od.Product.MainDiamond.Clarity,
                        od.Product.MainDiamond.CaratWeight,
                        CertificateId = od.Product.MainDiamond.Certificate.CertificateId
                    }).ToList()
                }
            }).ToList();

            // Logging to ensure the data is being loaded correctly
            Console.WriteLine("Fetched Warranties: " + Newtonsoft.Json.JsonConvert.SerializeObject(result));

            return Ok(result);
        }

        // PUT: api/Warranties/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarranty(int id, WarrantyDTO warrantyDTO)
        {
            if (id != warrantyDTO.WarrantyId)
            {
                return BadRequest();
            }

            var warranty = await _context.Warranties.FindAsync(id);
            if (warranty == null)
            {
                return NotFound();
            }

            warranty.WarrantyEndDate = warrantyDTO.WarrantyEndDate;
            warranty.StoreRepresentativeSignature = warrantyDTO.StoreRepresentativeSignature;

            _context.Entry(warranty).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarrantyExists(id))
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

        // DELETE: api/Warranties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarranty(int id)
        {
            var warranty = await _context.Warranties.FindAsync(id);
            if (warranty == null)
            {
                return NotFound();
            }

            _context.Warranties.Remove(warranty);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WarrantyExists(int id)
        {
            return _context.Warranties.Any(e => e.WarrantyId == id);
        }
    }

    // WarrantyDTO class for creating and updating warranties
    public class WarrantyDTO
    {
        public int WarrantyId { get; set; }
        public int OrderId { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public string StoreRepresentativeSignature { get; set; }
    }
}
