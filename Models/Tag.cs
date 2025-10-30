namespace NoteManagerDotNet.Models
{
    public class Tag
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public User? Owner { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class TagCreateDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TagReponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}