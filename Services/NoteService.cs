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

        public async Task<List<Note>> GetAllNotesAsync(long userId)
        {
            return await _context.Notes
                .Include(n => n.Tags)
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }

        public async Task<Note?> UpdateNoteAsync(long noteId, NoteUpdateDto noteDto, long userId)
        {
            var note = await _context.Notes
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);

            if (note == null)
            {
                return null;
            }

            if (noteDto.Title != null)
            {
                var titleExists = await _context.Notes.AnyAsync(n => n.UserId == userId && n.Title == noteDto.Title && n.Id != noteId);
                if (titleExists)
                {
                    return null;
                }
                note.Title = noteDto.Title;
            }
            if (noteDto.Content != null)
            {
                note.Content = noteDto.Content;
            }
            note.UpdatedAt = DateTime.UtcNow;

            if (noteDto.TagNames != null)
            {
                var tags = await _tagService.GetOrCreateTagsAsync(noteDto.TagNames, userId);
                note.Tags.Clear();
                note.Tags = tags;
            }

            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<bool> DeleteNoteAsync(long noteId, long userId)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);
            if (note == null)
            {
                return false;
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Note>> SearchNotesAsync(long userId, string? query, List<string>? tags)
        {
            var notesQuery = _context.Notes
                .Include(n => n.Tags)
                .Where(n => n.UserId == userId);

            if (!string.IsNullOrWhiteSpace(query))
            {
                string search = query.ToLower();
                notesQuery = notesQuery.Where(n =>
                    n.Title.ToLower().Contains(search) ||
                    n.Content.ToLower().Contains(search)
                );
            }

            if (tags != null && tags.Count > 0)
            {
                foreach (var tag in tags)
                {
                    notesQuery = notesQuery.Where(n =>
                        n.Tags.Any(t => t.Name.ToLower() == tag.ToLower())
                    );
                }
            }

            return await notesQuery
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();
        }
    }
}