using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // <-- ADD THIS

namespace NoteManagerDotNet.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        // ... (rest of the controller logic)

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            // You can access the authenticated user's details here:
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // This is the line that fixes the CS0161 error:
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                // ... (Original logic for generating weather data)
            })
            .ToArray();
        }
        
        // ... (rest of the controller)
    }
}