using System.ComponentModel.DataAnnotations;

namespace server.models.DTO;

public class ProgressRequest
{
    [Required]
    public string NextNode { get; set; }

    [Required]
    public int TurnsRemaining { get; set; } // backend should also track this?

    [Required]
    public string SummarySoFar { get; set; } // instead of this, we could just have an index of which node we're on (that would also mean we don't need NextNode)

    [Required]
    public string StoryStructure { get; set; } // could be got from the db

    [Required]
    public string UserAction { get; set; }

    public ProgressRequest() {}
}