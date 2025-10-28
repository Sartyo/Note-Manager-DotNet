// Services/IUserService.cs (Create this new file)
using NoteManagerDotNet.Models;

namespace NoteManagerDotNet.Services
{
    public interface IUserService
    {
        // Registration
        Task<User?> RegisterAsync(UserRegistrationDto userDto);
        // Login/Authentication
        Task<string?> AuthenticateAsync(UserLoginDto userDto); 
        // JWT generation
        string GenerateJwtToken(User user);
    }
}