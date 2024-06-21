using System;
using System.Collections.Generic;

namespace BE_V2.DataDB;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int? UserId { get; set; }

    public DateOnly DateJoined { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? User { get; set; }
}
