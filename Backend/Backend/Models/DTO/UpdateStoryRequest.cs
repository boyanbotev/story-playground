using System.ComponentModel.DataAnnotations;
using Backend.Models.Db;

namespace Backend.Models.DTO;

public class UpdateStoryRequest
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
    public StoryNode[] Nodes { get; set; }
}