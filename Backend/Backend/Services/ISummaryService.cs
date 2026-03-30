using Backend.Models.DTO;

public interface ISummaryService
{
    Task<string> GenerateSummary(ProgressRequest progressRequest, string storyText, CancellationToken cancellationToken);
}