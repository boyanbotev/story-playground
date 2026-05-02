using Backend.Models.DTO;

public interface ILLMService
{
    Task<LLMResponse> Generate(string prompt, CancellationToken cancellationToken);
}