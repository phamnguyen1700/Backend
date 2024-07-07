using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BE_V2.DataDB;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;

        public FeedbackController(DiamondShopV4Context context)
        {
            _context = context;
        }

        // GET: api/Feedback
        [HttpGet]
        public async Task<IActionResult> GetAllFeedback()
        {
            var feedbacks = await _context.Feedbacks.ToListAsync();

            if (feedbacks == null || feedbacks.Count == 0)
            {
                return NotFound(new { Message = "No feedback found." });
            }

            return Ok(feedbacks);
        }

        // GET: api/Feedback/{productId}
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetFeedbackByProductId(int productId)
        {
            var feedbacks = await _context.Feedbacks
                .Where(f => f.ProductId == productId)
                .ToListAsync();

            if (feedbacks == null || feedbacks.Count == 0)
            {
                return NotFound(new { Message = "No feedback found for this product." });
            }

            return Ok(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDTO createFeedbackDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CustomerId == createFeedbackDTO.CustomerId);

            if (customer == null || customer.User == null)
            {
                return BadRequest(new { Message = "Invalid customer or user." });
            }

            var feedback = new Feedback
            {
                ProductId = createFeedbackDTO.ProductId,
                CustomerId = createFeedbackDTO.CustomerId,
                FeedbackText = createFeedbackDTO.FeedbackText,
                Rating = createFeedbackDTO.Rating,
                FeedbackDate = createFeedbackDTO.FeedbackDate,
                UserName = customer.User.Name ?? customer.User.Username // Ensure this field is set correctly
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFeedbackByProductId), new { productId = feedback.ProductId }, feedback);
        }


    }
}
