using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_V2.DataDB
{
    public class Certificate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CertificateId { get; set; }

        public int DiamondId { get; set; }

        public DateTime ReportDate { get; set; }

        [MaxLength(50)]
        public string ReportNumber { get; set; }

        [MaxLength(100)]
        public string ClarityCharacteristics { get; set; }

        [MaxLength(100)]
        public string Inscription { get; set; }

        [MaxLength(255)]
        public string Comments { get; set; }

        [ForeignKey("DiamondId")]
        public virtual Diamond? Diamond { get; set; } // Make this optional
    }
}
