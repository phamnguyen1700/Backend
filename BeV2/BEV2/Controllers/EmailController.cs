using BE_V2.DataDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IConfiguration _configuration;

        public EmailController(ILogger<EmailController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("send-payment-confirmation")]
        public async Task<IActionResult> SendPaymentConfirmation([FromBody] PaymentConfirmationRequest request)
        {
            var smtpSettings = _configuration.GetSection("Smtp");
            var smtpServer = smtpSettings["Server"];
            var smtpPort = int.Parse(smtpSettings["Port"]);
            var smtpUsername = smtpSettings["Username"];
            var smtpPassword = smtpSettings["Password"];
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"]);

            var mail = new MailMessage();
            mail.To.Add(new MailAddress(request.Email));
            mail.From = new MailAddress(smtpUsername);
            mail.Subject = "Payment Confirmation";
            mail.Body = $@"
                Dear {request.CustomerName},
                Your payment for Order ID: {request.OrderId} has been successfully processed.
                Order Details:
                {string.Join("\n", request.OrderDetails.Select(od => $"Product: {od.ProductName}, Quantity: {od.Quantity}, Price: {od.ProductPrice}"))}
                Total Amount: ${request.TotalAmount}
                Deposit: ${request.Deposit}
                Amount Paid: ${request.AmountPaid}
                Thank you for shopping with us!
            ";
            mail.IsBodyHtml = false;

            var smtp = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = enableSsl
            };

            try
            {
                smtp.Send(mail);
                return Ok(new { message = "Email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending payment confirmation email");
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }

        public class PaymentConfirmationRequest
        {
            public string Email { get; set; }
            public int OrderId { get; set; }
            public string CustomerName { get; set; }
            public List<OrderDetail> OrderDetails { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal Deposit { get; set; }
            public decimal AmountPaid { get; set; }
        }

        public class OrderDetail
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal ProductPrice { get; set; }
        }
    }
}
