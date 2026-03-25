using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models.Db;

[JsonDerivedType(typeof(StoryNode), "story")]
[JsonDerivedType(typeof(QuestNode), "quest")]
public class Node
{
    [Key]
    public int Id { get; set; }
    public string Content { get; set; }
    public int StoryId { get; set; }
    [JsonIgnore]
    public Story story { get; set; }
    public int Order { get; set; }

    public Node() {}
}