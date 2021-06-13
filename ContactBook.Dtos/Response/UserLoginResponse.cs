using System;
using System.Collections.Generic;
using System.Text;

namespace ContactBook.Dtos.Response
{
    public class UserLoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
    }
}
