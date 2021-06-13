using ContactBook.Dtos.Request;
using ContactBook.Dtos.Response;
using ContactBook.Models;
using ContactBook.Utitlities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AppUserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuth _auth;

        public AppUserController(UserManager<AppUser> userManager,
                                  SignInManager<AppUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IAuth auth)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _auth = auth;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest model)
        {

            var existinguser = await _userManager.FindByEmailAsync(model.Email);
            if (existinguser != null)
            {
                return BadRequest("Email Already exist");

            }

            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
            };
            if (await _roleManager.FindByNameAsync("Regular User") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("Regular User"));
            }
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest("Something went Wrong");
            }
            await _userManager.AddToRoleAsync(user, "Regular User");
            var res = new UserRegisterResponse
            {
                Success = true,
                Id = user.Id,
                Message = "User Successfully Created"
            };
            return Ok(res);
        }

        [HttpPost]
        [Route("login")]

        public async Task<IActionResult> Login([FromBody] UserLoginRequest model)
        {
            var existinguser = await _userManager.FindByEmailAsync(model.Email);
            if (existinguser == null)
            {
                return NotFound("Invalid Credentials");
            }
            //var isCorrect = await _userManager.CheckPasswordAsync(existinguser, model.Password);
            var isCorrect = await _signInManager.PasswordSignInAsync(existinguser, model.Password, false, false);
            if (!isCorrect.Succeeded)
            {
                return BadRequest("Invalid Credentials");
            }

            var jwtToken = await _auth.GenerateJwtToken(existinguser);
            var res = new UserLoginResponse
            {
                Success = true,
                Token = jwtToken
            };

            return Ok(res);

        }

        [HttpPost]
        [Route("makeadmin/{id}")]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            if (await _roleManager.FindByNameAsync("Admin") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User does not exist");
            }
            var res = await _userManager.AddToRoleAsync(user, "Admin");

            return Ok("You are now an Admin");

        }
    }
}
