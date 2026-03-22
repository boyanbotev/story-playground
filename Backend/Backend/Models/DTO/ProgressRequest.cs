using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTO;

public class ProgressRequest
{
    [Required]
    public int StoryId { get; set; }

    [Required]
    public int NodeIndex { get; set; }

    [Required]
    public int TransitionTurnsRemaining { get; set; }
    
    [Required]
    public int ContentTurnsRemaining { get; set; }

    [Required]
    public string SummarySoFar { get; set; }

    [Required]
    public string UserAction { get; set; }

    public ProgressRequest() {}
}