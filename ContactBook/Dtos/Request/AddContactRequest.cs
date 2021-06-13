using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook.Dtos.Request
{
    public class AddContactRequest
    {
        [Required]
        [MinLength(4)]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }
        public IFormFile Photo { get; set; }
    }
}
