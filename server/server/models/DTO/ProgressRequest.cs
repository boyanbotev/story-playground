using System.ComponentModel.DataAnnotations;

namespace server.models.DTO;

public class ProgressRequest
{
    [Required]
    public int StoryId { get; set; }

    [Required]
    public int NodeIndex { get; set; }

    [Required]
    public int TurnsRemaining { get; set; } // backend should also track this?

    [Required]
    public string SummarySoFar { get; set; }

    [Required]
    public string UserAction { get; set; }

    public ProgressRequest() {}
}