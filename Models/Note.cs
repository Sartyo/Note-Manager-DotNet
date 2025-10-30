namespace NoteManagerDotNet.Models
{
    public class Note
    {
        public long Id { get; set; }
        public long UserId { get; set; }

        public User? Author { get; set; }
        public string Title { get; set; } = string.Empty;
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class NoteCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public ICollection<string> TagNames { get; set; } = new List<string>();
    }

    public class NoteUpdateDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public ICollection<string>? TagNames { get; set; }
    }

    public class NoteResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public ICollection<string> TagNames { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}