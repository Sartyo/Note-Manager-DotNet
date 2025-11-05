using NoteManagerDotNet.Models;

namespace NoteManagerDotNet.Services
{
    public interface INoteService
    {
        Task<Note?> CreateNoteAsync(NoteCreateDto noteDto, long userId);
        Task<Note?> GetNoteByIdAsync(long noteId, long userId);
        Task<List<Note>> GetAllNotesAsync(long userId);
        Task<Note?> UpdateNoteAsync(long noteId, NoteUpdateDto noteDto, long userId);
        Task<bool> DeleteNoteAsync(long noteId, long userId);
    }
}