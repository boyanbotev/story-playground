using System.ComponentModel.DataAnnotations;
using server.models.db;

namespace server.models.DTO;

public class UpdateStoryRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Structure { get; set; }

    [Required]
    public StoryNode[] Nodes { get; set; }
}