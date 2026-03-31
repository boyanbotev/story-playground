using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTO;


/// <summary>
/// Request for updating a story on the database.
/// </summary>
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
    public string MainCharacterName { get; set; }

    [Required]
    public List<NodeRequest> Nodes { get; set; }

    public UpdateStoryRequest() {}
}