using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_V2.DataDB
{
    public class PriceDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PriceDetailID { get; set; }

        public int ProductID { get; set; }

        public decimal DiamondPrice { get; set; }

        public decimal JewelryPrice { get; set; }

        public decimal ProcessingPrice { get; set; }

        public decimal Profit { get; set; }

        public virtual Product? Product { get; set; }
    }
}
