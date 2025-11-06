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

        public async Task<Tag?> GetTagByNameAsync(string tagName, long userId)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Name == tagName);
        }

        public async Task<Tag?> CreateTagAsync(TagCreateDto tagDto, long userId)
        {
            var tagExists = await _context.Tags.AnyAsync(t => t.UserId == userId && t.Name == tagDto.Name);
            if (tagExists)
            {
                return null; // Or throw an exception if preferred
            }

            var tag = new Tag
            {
                UserId = userId,
                Name = tagDto.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> UpdateTagAsync(long tagId, TagCreateDto tagDto, long userId)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == tagId && t.UserId == userId);
            if (tag == null)
            {
                return null;
            }

            tag.Name = tagDto.Name;
            tag.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<bool> DeleteTagAsync(long tagId, long userId)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == tagId && t.UserId == userId);
            if (tag == null)
            {
                return false;
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Tag>> GetAllTagsAsync(long userId)
        {
            return await _context.Tags
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(long tagId, long userId)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == tagId && t.UserId == userId);
        }
    }
}