using System;
using System.Collections.Generic;

namespace BE_V2.DataDB;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? OrderId { get; set; }

    public decimal? Deposit { get; set; }

    public decimal? AmountPaid { get; set; }

    public decimal? Total { get; set; }

    public DateOnly? DatePaid { get; set; }

    public virtual Order? Order { get; set; }
}
