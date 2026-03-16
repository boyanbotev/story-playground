using System.ComponentModel.DataAnnotations;

namespace server.models.db;

public class Story
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Structure { get; set; }
    public List<StoryNode> Nodes { get; set; }

    public Story() {}
}