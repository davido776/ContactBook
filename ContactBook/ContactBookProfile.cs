using AutoMapper;
using ContactBook.Dtos.Request;
using ContactBook.Dtos.Response;
using ContactBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook
{
    public class ContactBookProfile : Profile
    {

        public ContactBookProfile()
        {
            CreateMap<Contact, GetContactResponse>();
            CreateMap<AddContactRequest, Contact>();
            CreateMap<Contact, AddContactRequest>();
        }


    }
}
