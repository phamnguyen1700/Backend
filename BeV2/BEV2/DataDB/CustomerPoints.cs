using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_V2.DataDB
{
    public class CustomerPoints
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerPointID { get; set; }

        public int CustomerID { get; set; }

        public int Points { get; set; } = 0;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public virtual Customer? Customer { get; set; }
    }
}
