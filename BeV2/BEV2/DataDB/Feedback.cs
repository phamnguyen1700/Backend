using System;
using System.Collections.Generic;

namespace BE_V2.DataDB
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int? CustomerId { get; set; }
        public string? FeedbackText { get; set; }
        public int? Rating { get; set; }
        public DateOnly? FeedbackDate { get; set; }
        public int? ProductId { get; set; }

        public string UserName { get; set; } // Added property

        public virtual Customer? Customer { get; set; }
        public virtual Product? Product { get; set; }
    }
}
