using System;
using System.ComponentModel.DataAnnotations;

namespace BE_V2.DTOs
{
    public class CustomerPointsDTO
    {
        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int Points { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    public class PurchaseRequest
    {
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public decimal OrderTotal { get; set; }
        public bool UsePoints { get; set; }
    }
}
