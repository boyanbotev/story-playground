using System.ComponentModel.DataAnnotations;

namespace server.models.db;

public class StoryNode
{
    [Key]
    public int Id { get; set; }
    public int Turns { get; set; }
    public string Content { get; set; }
    public Story story { get; set; }

    public StoryNode() {}
}