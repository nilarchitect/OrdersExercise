using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrdersAPI.Controllers.RequestModels;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using OrdersAPI.Services.Contracts;
using OrdersAPI.Models;
using OrdersAPI.Models.DTOs;
using AutoMapper;

namespace OrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, 
                                IMapper mapper, 
                                IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login(RequestModels.LoginRequest request)
        {
            string role = null;

            //if (request.Username == "admin" && request.Password == "password")
            //    role= "Admin";
            //else if (request.Username == "user" && request.Password == "password")
            //    role= "User";
            //else
            //    return Unauthorized(new { message = "Invalid username or password" });
            User user = new Models.User();

            try
            {
                user = _userService.AuthenticateUser(request.Username, request.Password);
            }
            catch (NullReferenceException ne) 
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (ArgumentException ae)
            {
                return BadRequest(new { message = "Invalid username or password" });
            }

            //claims generation
            var claims = new[]
            {                   
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            //Generate the jwt token
            var jwtKey = _configuration["Jwt:Key"];

            var key=new SymmetricSecurityKey
                (System.Text.Encoding.UTF8.GetBytes(jwtKey));

            var credencials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credencials
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token=tokenString} );
        }
        [HttpPost("user")]
        public IActionResult CreateUser(CreateUserRequest request)
        {
            User user = _mapper.Map<CreateUserRequest, User>(request);
            try
            {
                _userService.CreateUser(user);
                return Ok(new { message = "User registered successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
