using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTO;

public class RegisterRequest
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string ApiKey { get; set; }

    public RegisterRequest() { }
}