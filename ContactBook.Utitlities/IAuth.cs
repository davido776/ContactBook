using ContactBook.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Utitlities
{
    public interface IAuth
    {
        Task<string> GenerateJwtToken(AppUser user);
    }
}
