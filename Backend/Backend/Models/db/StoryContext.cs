using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Db;

public class StoryContext : DbContext
{
    public DbSet<Story> Stories { get; set; }
    public DbSet<StoryNode> StoryNodes { get; set; }
    public string DbPath { get; }

    public StoryContext(DbContextOptions<StoryContext> options) : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "stories.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StoryNode>()
            .HasOne(sn => sn.story)
            .WithMany(s => s.Nodes);
    }
}