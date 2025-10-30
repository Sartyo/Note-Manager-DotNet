using NoteManagerDotNet.Models;

namespace NoteManagerDotNet.Services
{
    public interface ITagService
    {
        Task<List<Tag>> GetOrCreateTagsAsync(ICollection<string> tagNames, long userId);
    }
}