using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Db;

public class Story
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Structure { get; set; }
    public string StartingSummary { get; set; }
    public string Introduction { get; set; }
    public List<StoryNode> Nodes { get; set; }

    public Story() {}
}