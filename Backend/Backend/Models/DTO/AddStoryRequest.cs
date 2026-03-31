using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models.DTO;

/// <summary>
/// Request for adding a new story that will be saved to the database.
/// </summary>
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

/// <summary>
/// Base class for all node types.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StoryNodeRequest), "story")]
[JsonDerivedType(typeof(QuestNodeRequest), "quest")]
public abstract class NodeRequest
{

}

/// <summary>
/// Request for adding a story node with high degree of granularity (describing specific events).
/// </summary>
public class StoryNodeRequest : NodeRequest
{
    [Required]
    public int TransitionTurns { get; set; }

    [Required]
    public int ContentTurns { get; set; }

    [Required]
    public string Content { get; set; }
}

/// <summary>
/// Request for adding a story node that lets the user complete a task in their own way.
/// </summary>
public class QuestNodeRequest : NodeRequest
{
    [Required]
    public string UserGoal { get; set; }

    [Required]
    public string Difficulty { get; set; }
}