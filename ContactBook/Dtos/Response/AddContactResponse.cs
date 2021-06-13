using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook.Dtos.Response
{
    public class AddContactResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string ContactId { get; set; }
    }
}
