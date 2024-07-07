using BE_V2.DataDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

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

            var orderDetails = string.Join("<br>", request.OrderDetails.Select(od => $"<div class='product'><p>Product: {od.ProductName}</p><p>Quantity: {od.Quantity}</p><p>Price: ${od.ProductPrice}</p></div>"));

            // Read the HTML template from file
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "EmailTemplate.html");
            var emailTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

            // Replace placeholders with actual data
            var emailBody = emailTemplate
                .Replace("{OrderId}", request.OrderId.ToString())
                .Replace("{DayCreated}", DateTime.Now.ToString("yyyy-MM-dd"))
                .Replace("{OrderDetails}", orderDetails)
                .Replace("{CustomerName}", request.CustomerName)
                .Replace("{CustomerEmail}", request.Email)
                .Replace("{TotalAmount}", request.TotalAmount.ToString("F2"))
                .Replace("{Deposit}", request.Deposit.ToString("F2"))
                .Replace("{AmountPaid}", request.AmountPaid.ToString("F2"));

            var mail = new MailMessage();
            mail.To.Add(new MailAddress(request.Email));
            mail.From = new MailAddress(smtpUsername);
            mail.Subject = "Payment Confirmation";
            mail.Body = emailBody;
            mail.IsBodyHtml = true;

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

        [HttpPost("send-customer-support")]
        public IActionResult SendCustomerSupportEmail([FromBody] CustomerSupportRequest request)
        {
            var smtpSettings = _configuration.GetSection("Smtp");
            var smtpServer = smtpSettings["Server"];
            var smtpPort = int.Parse(smtpSettings["Port"]);
            var smtpUsername = smtpSettings["Username"];
            var smtpPassword = smtpSettings["Password"];
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"]);

            var mail = new MailMessage();
            mail.To.Add(new MailAddress(smtpUsername));
            mail.From = new MailAddress(smtpUsername);
            mail.Subject = $"Customer Support Inquiry from {request.CustomerName}";
            mail.Body = $"You have received a new customer support inquiry:\n\n" +
                        $"Customer Name: {request.CustomerName}\n" +
                        $"Customer Email: {request.CustomerEmail}\n" +
                        $"Phone Number: {request.PhoneNumber}\n\n" +
                        $"Message:\n{request.Message}\n\n" +
                        $"Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
            mail.IsBodyHtml = false;

            var smtp = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = enableSsl
            };

            try
            {
                smtp.Send(mail);
                return Ok(new { message = "Customer support email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending customer support email");
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

        public class CustomerSupportRequest
        {
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string PhoneNumber { get; set; }
            public string Message { get; set; }
        }

        public class OrderDetail
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal ProductPrice { get; set; }
        }
    }
}
