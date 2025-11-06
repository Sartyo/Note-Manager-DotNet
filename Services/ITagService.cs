using NoteManagerDotNet.Models;

namespace NoteManagerDotNet.Services
{
    public interface ITagService
    {
        Task<List<Tag>> GetOrCreateTagsAsync(ICollection<string> tagNames, long userId);
        Task<Tag?> GetTagByNameAsync(string tagName, long userId);
        Task<Tag?> CreateTagAsync(TagCreateDto tagDto, long userId);
        Task<Tag?> UpdateTagAsync(long tagId, TagCreateDto tagDto, long userId);
        Task<bool> DeleteTagAsync(long tagId, long userId);
        Task<List<Tag>> GetAllTagsAsync(long userId);
        Task<Tag?> GetTagByIdAsync(long tagId, long userId);
    }
}