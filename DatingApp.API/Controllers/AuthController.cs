using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        // private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthController(IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            // _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO)
        {
            // userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();

            // if (await _repo.UserExists(userForRegisterDTO.Username))
            //     return BadRequest("Username already exists");

            // // var userToCreate = new User
            // // {
            // //     Username = userForRegisterDTO.Username
            // // };

            // var userToCreate = _mapper.Map<User>(userForRegisterDTO);

            // var createdUser = await _repo.Register(userToCreate, userForRegisterDTO.Password);

            // var userToReturn = _mapper.Map<UserForDetailsDTO>(createdUser);

            // //return StatusCode (201);
            // return CreatedAtRoute("GetUser", new { controller = "Users", id = createdUser.Id }, userToReturn);

            var userToCreate = _mapper.Map<User>(userForRegisterDTO);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDTO.Password);

            var userToReturn = _mapper.Map<UserForDetailsDTO>(userToCreate);

            if (result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);
            }

            return BadRequest(result.Errors);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserforLoginDTO UserforLoginDTO)
        {
            // var userFromRepo = await _repo.Login(UserforLoginDTO.Username.ToLower(), UserforLoginDTO.Password);

            // if (userFromRepo == null)
            //     return Unauthorized();

            // var user = _mapper.Map<UserForListDTO>(userFromRepo);

            // return Ok(new
            // {
            //     token = GenerateJwtToken(userFromRepo),
            //     user
            // });

            var user = await _userManager.FindByNameAsync(UserforLoginDTO.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, UserforLoginDTO.Password, false);

            if (result.Succeeded)
            {
                var appUser = _mapper.Map<UserForListDTO>(user);

                return Ok(new
                {
                    // FIXMEOS -- Check this again.. in the tutorial, initially await keywork wasn't used, but I got issue
                    //      so I used await. And later in tutorial, in the SPA website, they got issue because they didn't 
                    //      '.Result' and they used it. But, I didn't get any error even though I didn't use '.Result'.
                    //      And when I used '.Result', still I don't find any issue. Not sure, what is going on here.

                    token = await GenerateJwtToken(user),
                    // token = GenerateJwtToken(user).Result,
                    user = appUser
                });
            }

            return Unauthorized();
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim> {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),
                new Claim (ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}