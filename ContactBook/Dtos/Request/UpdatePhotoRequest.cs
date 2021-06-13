using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook.Dtos.Request
{
    public class UpdatePhotoRequest
    {
        public IFormFile Photo { get; set; }
    }
}
