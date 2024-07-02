using System;
using System.ComponentModel.DataAnnotations;

namespace BE_V2.DataDB
{
    public class EventDTO
    {
        [Required]
        public string EventName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Description { get; set; }
    }
}
