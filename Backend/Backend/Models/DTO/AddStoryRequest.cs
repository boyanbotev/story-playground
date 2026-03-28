using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models.DTO;

public class AddStoryRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Structure { get; set; }
    [Required]
    public string StartingSummary { get; set; }
    [Required]
    public string Introduction { get; set; }
    [Required] 
    public string MainCharacterName { get; set; }
    [Required]
    public List<NodeRequest> Nodes { get; set; }

    public AddStoryRequest() {}
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StoryNodeRequest), "story")]
[JsonDerivedType(typeof(QuestNodeRequest), "quest")]
public abstract class NodeRequest
{

}

public class StoryNodeRequest : NodeRequest
{
    [Required]
    public int TransitionTurns { get; set; }

    [Required]
    public int ContentTurns { get; set; }

    [Required]
    public string Content { get; set; }
}

public class QuestNodeRequest : NodeRequest
{
    [Required]
    public string UserGoal { get; set; }

    [Required]
    public string Difficulty { get; set; }
}