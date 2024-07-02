using System.ComponentModel.DataAnnotations;

namespace BE_V2.DataDB
{
    public class PriceDetailDTO
    {
        public int PriceDetailID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public decimal DiamondPrice { get; set; }

        [Required]
        public decimal JewelryPrice { get; set; }

        [Required]
        public decimal ProcessingPrice { get; set; }

        [Required]
        public decimal Profit { get; set; }
    }
}
