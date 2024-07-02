using BE_V2.DataDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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

            _logger.LogInformation($"Payment completed successfully for Order ID: {paymentRequest.OrderId}");
            return Ok(new { message = "Payment completed successfully" });
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
