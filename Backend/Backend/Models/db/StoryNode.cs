namespace Backend.Models.Db;

/// <summary>
/// A more granular node in a story represeting a single event.
/// </summary>
public class StoryNode : Node
{
    public int TransitionTurns { get; set; }
    public int ContentTurns { get; set; }

    public string Content { get; set; }

    public StoryNode() {}
}