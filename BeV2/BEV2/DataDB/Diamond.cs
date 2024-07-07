using System;
using System.Collections.Generic;

namespace BE_V2.DataDB;

public partial class Diamond
{
    public int DiamondId { get; set; }

    public string? Shape { get; set; }

    public string? Cut { get; set; }

    public string? Color { get; set; }

    public string? Clarity { get; set; }

    public decimal? CaratWeight { get; set; }

    public string? Fluorescence { get; set; }

    public decimal? LengthWidthRatio { get; set; }

    public decimal? Depth { get; set; }

    public decimal? Tables { get; set; }

    public string? Symmetry { get; set; }

    public string? Girdle { get; set; }

    public string? Measurements { get; set; }

    public string? Origin { get; set; }

    public virtual Certificate? Certificate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
