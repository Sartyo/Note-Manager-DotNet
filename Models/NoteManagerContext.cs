using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NoteManagerDotNet.Models
{
    public class NoteManagerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Note> Notes { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public NoteManagerDbContext(DbContextOptions<NoteManagerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).HasMaxLength(255);
                entity.Property(u => u.Username).HasMaxLength(100);
                entity.HasIndex(u => u.CreatedAt);
                entity.HasIndex(u => u.UpdatedAt);
            });

            // --- NOTE CONFIGURATION ---
            modelBuilder.Entity<Note>(entity =>
            {
                // 1. **REQUIRED:** Composite Unique Index for (UserId, Title)
                entity.HasIndex(n => new { n.UserId, n.Title }).IsUnique(); // ADDED

                // 2. Relationship: Note (Child) to User (Parent)
                entity.HasOne(n => n.Author)
                    .WithMany(u => u.Notes)
                    .HasForeignKey(n => n.UserId);

                // 3. **REQUIRED:** Content Max Length (You specified 5000 previously)
                entity.Property(n => n.Content).HasMaxLength(5000); // ADDED

                // 4. MaxLength for Title (Existing)
                entity.Property(n => n.Title).HasMaxLength(200);

                // 5. **REQUIRED:** Many-to-Many Relationship with Tag
                // EF Core will automatically create the NoteTag join table.
                entity.HasMany(n => n.Tags) // from Note model
                    .WithMany(t => t.Notes); // to Tag model (Collection<Note> Notes)

                // Other indexes (CreatedAt, UpdatedAt)
                entity.HasIndex(n => n.CreatedAt);
                entity.HasIndex(n => n.UpdatedAt);
            });
            
            modelBuilder.Entity<Tag>(entity =>
            {
                // 🔑 COMPOSITE UNIQUE INDEX: UserId and Name
                // This ensures the combination of UserId and Name is unique across the table.
                // EF Core will enforce this constraint on the database level.
                entity.HasIndex(t => new { t.UserId, t.Name })
                    .IsUnique();

                // One-to-Many relationship (Tag to User)
                entity.HasOne(t => t.Owner)
                    .WithMany(u => u.Tags)
                    .HasForeignKey(t => t.UserId);

                entity.Property(t => t.Name).HasMaxLength(100);

                entity.HasIndex(t => t.CreatedAt);
                entity.HasIndex(t => t.UpdatedAt);
            });
        }
    }
}