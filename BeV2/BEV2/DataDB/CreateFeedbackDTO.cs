using System;
using System.ComponentModel.DataAnnotations;

namespace BE_V2.DataDB
{
    public class CreateFeedbackDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(1000)]
        public string FeedbackText { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public DateOnly FeedbackDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
    }
}
