using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook.Dtos.Response
{
    public class UserRegisterResponse
    {
        public bool Success { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
    }
}
