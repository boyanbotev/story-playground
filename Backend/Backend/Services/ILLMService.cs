using Backend.Models.DTO;

public interface ILLMService
{
    Task<LLMResponse> Generate(string prompt, string apiKey, CancellationToken cancellationToken);
}