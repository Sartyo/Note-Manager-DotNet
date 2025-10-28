using NoteManagerDotNet.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace NoteManagerDotNet.Services
{
    public class UserService : IUserService
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;

        public UserService(UserContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- Helper Methods for Password Hashing ---
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }
        
        // --- Business Logic Methods ---
        public async Task<User?> RegisterAsync(UserRegistrationDto userDto)
        {
            // Check if username or email already exists
            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username || u.Email == userDto.Email))
            {
                return null; // Registration failed (user exists)
            }
            
            // Hash password
            CreatePasswordHash(userDto.Password, out byte[] hash, out byte[] salt);
            
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = hash,
                PasswordSalt = salt
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return user;
        }

        public async Task<string?> AuthenticateAsync(UserLoginDto userDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userDto.Username);
            
            // User not found
            if (user == null) return null;
            
            // Verify password
            if (!VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null; // Incorrect password
            }

            // Authentication successful, generate JWT
            return GenerateJwtToken(user);
        }
        
        // --- JWT Generation ---
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            
            // Claims contain information about the subject (user) and properties
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role) // Include user role
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Token validity
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}