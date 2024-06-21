using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BE_V2.DataDB;

public partial class OrderDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public string? ProductName { get; set; }

    public decimal? ProductPrice { get; set; }

    public int? Quantity { get; set; }

    [JsonIgnore]
    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
