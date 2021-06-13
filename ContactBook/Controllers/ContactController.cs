using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ContactBook.Data;
using ContactBook.Dtos.Request;
using ContactBook.Dtos.Response;
using ContactBook.Filters;
//using ContactBook.Dtos.Request;
//using ContactBook.Dtos.Response;
using ContactBook.Models;
using ContactBook.Wrappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContactBook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContactController : ControllerBase
    {
        private readonly ContactBookDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly Cloudinary _cloudinary;

        public ContactController(ContactBookDbContext context,
                                IMapper mapper,
                                IConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
            Account account = new Account(
            _config.GetSection("CloudinarySettings:CloudName").Value,
            _config.GetSection("CloudinarySettings:ApiKey").Value,
            _config.GetSection("CloudinarySettings:ApiSecret").Value);

            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("add")]
        public async Task<IActionResult> AddContact([FromForm] AddContactRequest model)
        {
            string photoName = null;
            var file = model.Photo;
            if (file != null)
            {

                //var folder = Path.Combine(IWebHost., "images");
                
                var imageUploadResult = new ImageUploadResult();
                using (var fs = file.OpenReadStream())
                {
                    var imageUploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, fs),
                        Transformation = new Transformation().Width(300).Height(300)
                    .Crop("fill").Gravity("face")
                    };
                    imageUploadResult = _cloudinary.Upload(imageUploadParams);
                }
                var publicId = imageUploadResult.PublicId;
                photoName = imageUploadResult.Url.ToString();
                
            }


            /*var newcontact = new Contact
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                PhotoUrl = photoName
            };*/

            var newcontact = _mapper.Map<AddContactRequest,Contact>(model);
            newcontact.PhotoUrl = photoName;

            await _context.Contacts.AddAsync(newcontact);
            var save = await _context.SaveChangesAsync();
            AddContactResponse res = null;

            if (save > 0)
            {
                res = new AddContactResponse
                {
                    Success = true,
                    Message = "Contact Successfully Added",
                    ContactId = newcontact.Id
                };
            }


            return Ok(res);

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Regular User")]
        //[Route("/{id}")]

        public async Task<IActionResult> GetContact(string id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("Contact Not Found");
            }
            var res1 = _mapper.Map<GetContactResponse>(contact);
            var res = new Response<GetContactResponse>(res1);
            return Ok(res);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("all-contacts")]
        public async Task<IActionResult> GetAllConatcts([FromQuery] PaginationFilter filter)
        {
            //var contacts = await _context.Contacts.ToListAsync();
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Contacts
                                   .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                                   .Take(validFilter.PageSize)
                                   .ToListAsync();
            if (pagedData.Count == 0)
            {
                return NotFound("No contact Found");
            }

            List<Contact> contacts = _mapper.Map<List<Contact>>(pagedData);
            //GetContactResponse[] contacts = _mapper.Map<GetContactResponse[]>(pagedData);
            var res = new PagedResponse<List<Contact>>(contacts, validFilter.PageNumber, validFilter.PageSize);
            
            return Ok(res);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("update/{id}")]

        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactRequest model, string id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("Contact does not exist");
            }

            contact.FirstName = model.FirstName;
            contact.LastName = model.LastName;
            contact.PhoneNumber = model.PhoneNumber;
            contact.Address = model.Address;
            contact.Email = model.Email;

           
            _context.Contacts.Update(contact);
            var save = await _context.SaveChangesAsync();

            if (save < 1)
            {
                return BadRequest("Something went wrong");
            }

            return Ok("Contact Successfully Updated");
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("delete/{id}")]

        public async Task<IActionResult> DeleteContact(string id)
        {
            var contact = _context.Contacts.Find(id);

            if (contact == null)
            {
                return NotFound("Contact does not exist");
            }

            _context.Contacts.Remove(contact);
            var delete = await _context.SaveChangesAsync();
            if (delete < 1)
            {
                return BadRequest("Something went wrong");
            }

            return Ok("Contact successfully deleted");


        }

        [HttpPatch]
        [Authorize(Roles = "Admin, Regular User")]
        [Route("update-photo/{id}")]

        public async Task<IActionResult> UpdatePhoto([FromForm] UpdatePhotoRequest model, string id)
        {
            if (model.Photo == null)
            {
                return BadRequest("No Image Selected");
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("contact does not exist");
            }

           
            string photoName = null;

            var file = model.Photo;
            var imageUploadResult = new ImageUploadResult();
            using (var fs = file.OpenReadStream())
            {
                var imageUploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, fs),
                    Transformation = new Transformation().Width(300).Height(300)
                .Crop("fill").Gravity("face")
                };
                imageUploadResult = _cloudinary.Upload(imageUploadParams);
            }
            var publicId = imageUploadResult.PublicId;
            photoName = imageUploadResult.Url.ToString();


            contact.PhotoUrl = photoName;
            _context.Update(contact);
            await _context.SaveChangesAsync();
            return Ok("photo Updated");
        }



        [HttpGet]
        [Route("search")]
        public IActionResult GetContactsByTerm(string term = null)
        {

            if (term == null)
            {
                return BadRequest("No search term");
            }
            var contacts = _context.Contacts.Where(c => c.FirstName.StartsWith(term) || c.LastName.StartsWith(term)).OrderBy(c => c.FirstName).ToList();
            if (contacts.Count == 0)
            {
                return NotFound($"No Contact with name {term} exist");
            }
            GetContactResponse[] res = _mapper.Map<GetContactResponse[]>(contacts);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Regular User")]
        //[Route("getbyemail/{email}")]

        public IActionResult GetContactByEmail(string email)
        {
            var contact = _context.Contacts.FirstOrDefault(c => c.Email == email);
            if (contact == null)
            {
                return NotFound("Contact Not Found");
            }
            var res = _mapper.Map<GetContactResponse>(contact);
            return Ok(res);
        }




    }
}
