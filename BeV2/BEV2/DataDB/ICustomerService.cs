using BE_V2.DTOs;
using System.Threading.Tasks;

namespace BE_V2.Services
{
    public interface ICustomerService
    {
        Task<int> AddPointsAsync(CustomerPointsDTO pointsDTO);
        Task<decimal> ApplyDiscountAsync(int customerId, decimal orderTotal, bool usePoints);
    }
}
