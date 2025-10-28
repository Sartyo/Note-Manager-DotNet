using Microsoft.AspNetCore.Mvc;
using NoteManagerDotNet.Models;
using NoteManagerDotNet.Services;

namespace NoteManagerDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto userDto)
        {
            var user = await _userService.RegisterAsync(userDto);
            
            if (user == null)
            {
                return BadRequest("Username or Email is already taken.");
            }
            
            return Ok(new { Message = "Registration successful!" });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            var token = await _userService.AuthenticateAsync(userDto);
            
            if (token == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            
            // Return the JWT token
            return Ok(new { Token = token });
        }
    }
}