using NoteManagerDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace NoteManagerDotNet.Services
{
    public class NoteService : INoteService
    {
        private readonly NoteManagerDbContext _context;
        private readonly ITagService _tagService;

        public NoteService(NoteManagerDbContext context, ITagService tagService)
        {
            _context = context;
            _tagService = tagService;
        }

        public async Task<Note?> CreateNoteAsync(NoteCreateDto noteDto, long userId)
        {
            var titleExists = await _context.Notes.AnyAsync(n => n.UserId == userId && n.Title == noteDto.Title);
            if (titleExists)
            {
                return null; // Or throw an exception if preferred
            }
            var tags = await _tagService.GetOrCreateTagsAsync(noteDto.TagNames, userId);

            var note = new Note
            {
                Title = noteDto.Title,
                Content = noteDto.Content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Tags = tags
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<Note?> GetNoteByIdAsync(long noteId, long userId)
        {
            return await _context.Notes
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);
        }
    }
}