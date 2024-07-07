using BE_V2.DataDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(DiamondShopV4Context context, ILogger<PaymentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("complete-payment")]
        public async Task<IActionResult> CompletePayment([FromBody] PaymentRequest paymentRequest)
        {
            var payment = new Payment
            {
                OrderId = paymentRequest.OrderId,
                Deposit = paymentRequest.Deposit,
                AmountPaid = paymentRequest.AmountPaid,
                Total = paymentRequest.Total,
                DatePaid = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Update Phase1 in OrderLog
            var orderLog = await _context.OrderLogs.FirstOrDefaultAsync(log => log.OrderID == paymentRequest.OrderId);
            if (orderLog != null)
            {
                orderLog.Phase1 = true;
                orderLog.TimePhase1 = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation($"Payment completed successfully for Order ID: {paymentRequest.OrderId}");
            return Ok(new { message = "Payment completed successfully" });
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(int orderId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _context.Payments.ToListAsync();
            return Ok(payments);
        }

        public class PaymentRequest
        {
            public int OrderId { get; set; }
            public decimal Deposit { get; set; }
            public decimal AmountPaid { get; set; }
            public decimal Total { get; set; }
        }
    }
}
