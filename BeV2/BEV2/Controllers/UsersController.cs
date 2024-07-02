using BE_V2.DataDB;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DiamondShopV4Context _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsersController> _logger;
        private readonly IValidator<User> _userValidator;
        private static readonly Dictionary<string, string> OtpStore = new Dictionary<string, string>();

        public UsersController(DiamondShopV4Context context, IConfiguration configuration, ILogger<UsersController> logger, IValidator<User> userValidator)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _userValidator = userValidator;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("User ID claim is missing");
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest("Invalid User ID claim");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest(new List<FluentValidation.Results.ValidationFailure>
                {
                    new FluentValidation.Results.ValidationFailure(nameof(user.Username), "Username already exists")
                });
            }

            var validationResult = await _userValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetUser", new { id = user.UserId }, user);
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest("User ID mismatch");
            }

            var validationResult = await _userValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            _context.Entry(existingUser).State = EntityState.Detached;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // PUT: api/Users/me
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> PutCurrentUser(User user)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("User ID claim is missing");
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest("Invalid User ID claim");
            }

            if (userId != user.UserId)
            {
                return BadRequest();
            }

            var validationResult = await _userValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userId))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Find associated customers
            var customers = await _context.Customers.Where(c => c.UserId == id).ToListAsync();
            if (customers.Any())
            {
                foreach (var customer in customers)
                {
                    // Find associated orders
                    var orders = await _context.Orders.Where(o => o.CustomerId == customer.CustomerId).ToListAsync();
                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            // Find and delete associated order details
                            var orderDetails = await _context.OrderDetails.Where(od => od.OrderId == order.OrderId).ToListAsync();
                            if (orderDetails.Any())
                            {
                                _context.OrderDetails.RemoveRange(orderDetails);
                            }
                        }
                        _context.Orders.RemoveRange(orders);
                    }
                }
                _context.Customers.RemoveRange(customers);
            }

            // Delete the user
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            _logger.LogInformation("Login attempt for user: {Username}", loginModel.Username);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginModel.Username);

            if (user == null)
            {
                _logger.LogWarning("Login failed for user: {Username} - Username does not exist", loginModel.Username);
                return Unauthorized(new ErrorResponse { Message = "Username does not exist" });
            }

            if (user.Password != loginModel.Password)
            {
                _logger.LogWarning("Login failed for user: {Username} - Incorrect password", loginModel.Username);
                return Unauthorized(new ErrorResponse { Message = "Incorrect password" });
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserId.ToString()),
                        new Claim(ClaimTypes.Role, user.RoleId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation("Login successful for user: {Username}", loginModel.Username);
                return Ok(new { Token = tokenString, RoleId = user.RoleId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the JWT token for user: {Username}", loginModel.Username);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            OtpStore[request.Email] = otp;

            _logger.LogInformation($"Stored OTP for {request.Email}: {otp}");

            var smtpSettings = _configuration.GetSection("Smtp");
            var smtpServer = smtpSettings["Server"];
            var smtpPort = int.Parse(smtpSettings["Port"]);
            var smtpUsername = smtpSettings["Username"];
            var smtpPassword = smtpSettings["Password"];
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"]);

            var mail = new MailMessage();
            mail.To.Add(new MailAddress(request.Email));
            mail.From = new MailAddress(smtpUsername);
            mail.Subject = "Your OTP Code";
            mail.Body = $"Your OTP code is {otp}";

            var smtp = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = enableSsl
            };

            try
            {
                smtp.Send(mail);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP email");
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] OtpVerificationRequest request)
        {
            _logger.LogInformation($"Verifying OTP for email: {request.Email}");

            if (OtpStore.TryGetValue(request.Email, out var storedOtp))
            {
                _logger.LogInformation($"Stored OTP: {storedOtp}");
                _logger.LogInformation($"Received OTP: {request.Otp}");

                if (storedOtp == request.Otp)
                {
                    // OTP verified successfully, remove it from the store
                    OtpStore.Remove(request.Email);
                    return Ok(new { message = "OTP verified successfully" });
                }
                else
                {
                    return BadRequest(new { error = "Invalid OTP" });
                }
            }
            else
            {
                return BadRequest(new { error = "OTP not found" });
            }
        }

        [HttpGet("current-session")]
        public IActionResult GetCurrentSession()
        {
            return Ok(new { message = "Session endpoint not needed with OTPStore approach" });
        }

        [HttpPost("complete-registration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationRequest request)
        {
            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
                Email = request.Email,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                Sex = request.Sex,
                DateOfBirth = request.DateOfBirth,
                RoleId = 5 // Default RoleId for new customers
            };

            var validationResult = await _userValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();

                // Add to Customer table
                var customer = new Customer
                {
                    UserId = user.UserId,
                    DateJoined = DateOnly.FromDateTime(DateTime.Now)
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUser", new { id = user.UserId }, user);
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(roles);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public class Role
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
        }


        public class ErrorResponse
        {
            public string Message { get; set; }
        }

        public class OtpRequest
        {
            public string Email { get; set; }
        }

        public class OtpVerificationRequest
        {
            public string Email { get; set; }
            public string Otp { get; set; }
        }

        public class CompleteRegistrationRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public string Sex { get; set; }
            public DateTime DateOfBirth { get; set; }
        }
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
