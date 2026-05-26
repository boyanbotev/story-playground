using Backend.Models.DTO;

public interface ISummaryService
{
    Task<LLMResponse> GenerateSummary(ProgressRequest progressRequest, string storyText, string apiKey, CancellationToken cancellationToken);
}