using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTO;

/// <summary>
/// Request for getting a turn's progress according to the user's action and story state.
/// </summary>
public class ProgressRequest
{
    [Required]
    public int StoryId { get; set; }

    [Required]
    public int NodeIndex { get; set; }

    public int TransitionTurnsRemaining { get; set; }
    
    public int ContentTurnsRemaining { get; set; }

    [Required]
    public string SummarySoFar { get; set; }

    [Required]
    public string UserAction { get; set; }

    public ProgressRequest() {}
}
