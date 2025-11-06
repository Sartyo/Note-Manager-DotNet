using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteManagerDotNet.Models;
using NoteManagerDotNet.Services;
using System.Security.Claims;

namespace NoteManagerDotNet.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagCreateDto tagDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out long userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            if (string.IsNullOrEmpty(tagDto.Name))
            {
                return BadRequest("Tag name is required.");
            }

            var tag = await _tagService.CreateTagAsync(tagDto, userId);
            if (tag == null)
            {
                return Conflict("A tag with the same name already exists.");
            }

            return CreatedAtRoute("GetTag", new { id = tag.Id }, tag);
        }

        [HttpGet("{id}", Name = "GetTag")]
        public async Task<IActionResult> GetTag(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out long userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var tag = await _tagService.GetTagByIdAsync(id, userId);
            if (tag == null)
            {
                return NotFound("Tag not found.");
            }

            return Ok(tag);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out long userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var tags = await _tagService.GetAllTagsAsync(userId);
            return Ok(tags);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(long id, [FromBody] TagCreateDto tagDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out long userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            if (string.IsNullOrEmpty(tagDto.Name))
            {
                return BadRequest("Tag name is required.");
            }

            var tag = await _tagService.UpdateTagAsync(id, tagDto, userId);
            if (tag == null)
            {
                return NotFound("Tag not found or name already in use.");
            }

            return Ok(tag);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out long userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var success = await _tagService.DeleteTagAsync(id, userId);
            if (!success)
            {
                return NotFound("Tag not found.");
            }

            return NoContent();
        }
    }
}