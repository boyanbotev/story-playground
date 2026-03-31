/// <summary>
/// Response for getting a turn's progress according to the user's action and story state.
/// </summary>
public class ProgressResponse
{
    public string SummarySoFar { get; set; }
    public int NodeIndex { get; set; }
    public int? TransitionTurnsRemaining { get; set; }
    public int? ContentTurnsRemaining { get; set; }
    public string UserGoal { get; set; }
    public string Difficulty { get; set; }
    public string QuestCompleteText { get; set; }
    public string StoryText { get; set; }
    public bool Completed { get; set; }
    public string? Error { get; set; }
}