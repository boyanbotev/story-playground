namespace Backend.Models.DTO;

public class LLMResponse
{
    public string? Text;
    public readonly string? Error;
    public LLMResponse(string text, string error)
    {
        Text = text;
        Error = error;
    }
}