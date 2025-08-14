using Microsoft.EntityFrameworkCore;
using MemoApp.Core.Entities;

namespace MemoApp.Infrastructure.Data;

public class MemoDbContext : DbContext
{
    public MemoDbContext(DbContextOptions<MemoDbContext> options) : base(options)
    {
    }

    public DbSet<Memo> Memos { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Memo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<Memo>()
            .HasMany(m => m.Tags)
            .WithMany(t => t.Memos)
            .UsingEntity(j => j.ToTable("MemoTags"));
    }
}