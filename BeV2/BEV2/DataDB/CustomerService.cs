using BE_V2.DataDB;
using BE_V2.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BE_V2.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DiamondShopV4Context _context;

        public CustomerService(DiamondShopV4Context context)
        {
            _context = context;
        }

        public async Task<int> AddPointsAsync(CustomerPointsDTO pointsDTO)
        {
            var customer = await _context.Customers.FindAsync(pointsDTO.CustomerID);
            if (customer == null)
                throw new Exception("Customer not found");

            var customerPoints = new CustomerPoints
            {
                CustomerID = pointsDTO.CustomerID,
                Points = pointsDTO.Points,
                LastUpdated = pointsDTO.LastUpdated
            };

            _context.CustomerPoints.Add(customerPoints);
            await _context.SaveChangesAsync();

            return customerPoints.CustomerPointID;
        }

        public async Task<decimal> ApplyDiscountAsync(int customerId, decimal orderTotal, bool usePoints)
        {
            if (!usePoints)
            {
                return orderTotal;
            }

            var customerPoints = await _context.CustomerPoints.FirstOrDefaultAsync(cp => cp.CustomerID == customerId);
            if (customerPoints == null || customerPoints.Points == 0)
            {
                throw new Exception("No points available");
            }

            var discount = customerPoints.Points * 0.25m / 100;
            var discountedTotal = orderTotal * (1 - discount);

            customerPoints.Points = 0;
            _context.CustomerPoints.Update(customerPoints);
            await _context.SaveChangesAsync();

            return discountedTotal;
        }

        public async Task<CustomerPointsDTO> GetCustomerPointsAsync(int customerId)
        {
            var customerPoints = await _context.CustomerPoints.FirstOrDefaultAsync(cp => cp.CustomerID == customerId);
            if (customerPoints == null)
                return null;

            return new CustomerPointsDTO
            {
                CustomerID = customerPoints.CustomerID,
                Points = customerPoints.Points,
                LastUpdated = customerPoints.LastUpdated
            };
        }
    }
}
