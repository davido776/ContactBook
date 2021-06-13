using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContactBook.Dtos.Request
{
    public class AddContactRequest
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        //public IFormFile Photo { get; set; }
    }
}
