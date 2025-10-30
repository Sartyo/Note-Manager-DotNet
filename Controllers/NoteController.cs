using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteManagerDotNet.Models;
using NoteManagerDotNet.Services;

namespace NoteManagerDotNet.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] NoteCreateDto noteDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out long userId))
            {
                return Unauthorized("Invalid user ID.");
            }
            var note = await _noteService.CreateNoteAsync(noteDto, userId);
            var responseDto = new NoteResponseDto
            {
                Id = note?.Id ?? 0,
                Title = note?.Title ?? string.Empty,
                Content = note?.Content ?? string.Empty,
                CreatedAt = note?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = note?.UpdatedAt ?? DateTime.UtcNow,
                TagNames = note?.Tags.Select(t => t.Name).ToList() ?? new List<string>()
            };
            return CreatedAtRoute("GetNote", new { id = note?.Id }, responseDto);
        }

        [HttpGet("{id}", Name = "GetNote")]
        public async Task<IActionResult> GetNote(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out long userId))
            {
                return Unauthorized("Invalid user ID.");
            }
            var note = await _noteService.GetNoteByIdAsync(id, userId);
            if (note == null)
            {
                return NotFound("Note not found.");
            }
            var responseDto = new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                TagNames = note.Tags.Select(t => t.Name).ToList()
            };
            return Ok(responseDto);
        }

    }
}