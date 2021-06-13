using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactBook.Dtos.Response
{
    public class AddContactResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string ContactId { get; set; }
    }
}
