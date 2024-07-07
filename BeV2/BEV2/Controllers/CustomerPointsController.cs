using BE_V2.DTOs;
using BE_V2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerPointsController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerPointsController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
        {
            try
            {
                // Áp dụng chiết khấu nếu khách hàng muốn sử dụng điểm
                var discountedTotal = await _customerService.ApplyDiscountAsync(request.CustomerId, request.OrderTotal, request.UsePoints);

                // Cập nhật điểm thưởng cho khách hàng
                var pointsDTO = new CustomerPointsDTO
                {
                    CustomerID = request.CustomerId,
                    Points = request.Quantity // Giả sử mỗi sản phẩm mua sẽ tích 1 điểm
                };
                await _customerService.AddPointsAsync(pointsDTO);

                return Ok(new { message = "Purchase completed successfully", discountedTotal });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("points/{customerId}")]
        public async Task<IActionResult> GetCustomerPoints(int customerId)
        {
            try
            {
                var points = await _customerService.GetCustomerPointsAsync(customerId);
                if (points == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }
                return Ok(points);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("add-points")]
        public async Task<IActionResult> AddPoints([FromBody] ManualPointsRequest request)
        {
            try
            {
                var pointsDTO = new CustomerPointsDTO
                {
                    CustomerID = request.CustomerId,
                    Points = request.Points,
                    LastUpdated = DateTime.Now
                };
                await _customerService.AddPointsAsync(pointsDTO);

                return Ok(new { message = "Points added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class ManualPointsRequest
    {
        public int CustomerId { get; set; }
        public int Points { get; set; }
    }
}
