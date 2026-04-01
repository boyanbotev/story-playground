using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Db;

public class Story
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Structure { get; set; }
    public string StartingSummary { get; set; }
    public string Introduction { get; set; }
    public string MainCharacterName { get; set; }
    public List<Node> Nodes { get; set; }
    public string UserId { get; set; }
    [ForeignKey(nameof(UserId))] 
    public User User { get; set; }

    public Story() {}
}