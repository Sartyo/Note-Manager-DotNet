using Microsoft.EntityFrameworkCore;
using NoteManagerDotNet.Models;

namespace NoteManagerDotNet.Services
{
    public class TagService: ITagService
    {
        private readonly NoteManagerDbContext _context;

        public TagService(NoteManagerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetOrCreateTagsAsync(ICollection<string> tagNames, long userId)
        {
            var existingTags = await _context.Tags
                .Where(t => t.UserId == userId && tagNames.Contains(t.Name))
                .ToListAsync();

            var existingTagNames = existingTags.Select(t => t.Name).ToHashSet();
            var newTagNames = tagNames.Where(name => !existingTagNames.Contains(name)).ToList();

            var newTags = newTagNames.Select(name => new Tag
            {
                UserId = userId,
                Name = name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await _context.Tags.AddRangeAsync(newTags);
            return existingTags.Concat(newTags).ToList();
        } 
    }
}