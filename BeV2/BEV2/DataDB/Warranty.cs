using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_V2.DataDB
{
    public class Warranty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WarrantyId { get; set; }

        public int OrderId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime WarrantyEndDate { get; set; }

        public string StoreRepresentativeSignature { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}
