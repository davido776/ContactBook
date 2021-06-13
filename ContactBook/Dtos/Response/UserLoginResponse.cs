using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook.Dtos.Response
{
    public class UserLoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
    }
}
