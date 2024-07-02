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
}
