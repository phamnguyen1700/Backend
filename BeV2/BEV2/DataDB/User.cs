using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_V2.DataDB
{
    public partial class User
    {
        public User()
        {
            Carts = new HashSet<Cart>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public int? RoleId { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        public string? Name { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Sex { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public virtual Customer? Customer { get; set; }

        public virtual Role? Role { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
    }
}
