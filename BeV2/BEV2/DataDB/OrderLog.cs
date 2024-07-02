using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_V2.DataDB;

public class OrderLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LogID { get; set; }

    public int OrderID { get; set; }

    public bool Phase1 { get; set; } = false;

    public bool Phase2 { get; set; } = false;

    public bool Phase3 { get; set; } = false;

    public bool Phase4 { get; set; } = false;

    public DateTime TimePhase1 { get; set; } = DateTime.UtcNow;

    public DateTime TimePhase2 { get; set; } = DateTime.UtcNow;
    public DateTime TimePhase3 { get; set; } = DateTime.UtcNow;
    public DateTime TimePhase4 { get; set; } = DateTime.UtcNow;


    public virtual Order? Order { get; set; }
}
