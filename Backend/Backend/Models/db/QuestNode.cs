namespace Backend.Models.Db;

/// <summary>
/// A less granular node in a story representing a free-form section where the user proceeds to predefined goal at their own pace.
/// </summary>
public class QuestNode : Node
{
    public string UserGoal { get; set; }
    public string Difficulty { get; set; }

    public QuestNode() {}
}