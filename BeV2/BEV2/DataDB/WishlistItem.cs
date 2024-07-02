using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_V2.DataDB
{
    public partial class WishlistItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WishlistItemId { get; set; }

        [Required]
        public int WishlistId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public DateTime AddedDate { get; set; }

        [ForeignKey("WishlistId")]
        public virtual Wishlist Wishlist { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
