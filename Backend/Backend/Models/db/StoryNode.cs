using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models.Db;

/// <summary>
/// A more granular node in a story represeting a single event.
/// </summary>
public class StoryNode : Node
{
    public int TransitionTurns { get; set; }
    public int ContentTurns { get; set; }

    public StoryNode() {}
}