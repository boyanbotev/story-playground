using System.ComponentModel.DataAnnotations;

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
    public List<StoryNodeRequest> Nodes { get; set; }

    public AddStoryRequest() {}
}

public class StoryNodeRequest
{
    [Required]
    public string Content { get; set; }
    [Required]
    public int TransitionTurns { get; set; }
    [Required]
    public int ContentTurns { get; set; }
}