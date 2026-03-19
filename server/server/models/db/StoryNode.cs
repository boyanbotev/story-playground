using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace server.models.db;

public class StoryNode
{
    [Key]
    public int Id { get; set; }
    public int Turns { get; set; }
    public string Content { get; set; }
    [JsonIgnore]
    public Story story { get; set; }

    public StoryNode() {}
}