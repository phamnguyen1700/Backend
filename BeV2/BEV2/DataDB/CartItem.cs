using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_V2.DataDB
{
    public partial class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemID { get; set; }

        [Required]
        public int CartID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [ForeignKey("CartID")]
        public virtual Cart Cart { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}
