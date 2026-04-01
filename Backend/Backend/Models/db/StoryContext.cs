using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Backend.Models.Db;

public class StoryContext : IdentityDbContext
{
    public DbSet<Story> Stories { get; set; }
    public DbSet<Node> Nodes { get; set; }
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

        modelBuilder.Entity<Node>()
            .HasOne(sn => sn.story)
            .WithMany(s => s.Nodes)
            .HasForeignKey(sn => sn.StoryId);

        modelBuilder.Entity<Node>()
            .HasDiscriminator<string>("NodeType")
            .HasValue<Node>("Node")
            .HasValue<StoryNode>("StoryNode")
            .HasValue<QuestNode>("QuestNode");

        modelBuilder.Entity<Story>()
            .HasOne(p => p.User)
            .WithMany(u => u.Stories)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}