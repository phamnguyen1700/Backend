using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_V2.DataDB
{
    public partial class Event
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Description { get; set; }

        public virtual ICollection<EventItem> EventItems { get; set; } = new List<EventItem>();
    }
}
