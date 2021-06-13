using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContactBook.Models
{
    public class Contact
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        
        public string PhotoUrl { get; set; }
    }
}
