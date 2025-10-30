using NoteManagerDotNet.Models;

namespace NoteManagerDotNet.Services
{
    public interface INoteService
    {
        Task<Note?> CreateNoteAsync(NoteCreateDto noteDto, long userId);
        Task<Note?> GetNoteByIdAsync(long noteId, long userId);
    }
}