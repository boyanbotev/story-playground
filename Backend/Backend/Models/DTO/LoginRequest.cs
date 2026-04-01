using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTO;

public class LoginRequest
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }

    public LoginRequest() { }
}