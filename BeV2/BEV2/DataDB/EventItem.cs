using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_V2.DataDB
{
    public partial class EventItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventItemID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal Discount { get; set; }

        [ForeignKey("EventID")]
        public virtual Event Event { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}
