using Backend.Services;

namespace Backend.Models.DTO;

public class ValidationResponse
{
    public ValidationResult result;
    public string? error;
}