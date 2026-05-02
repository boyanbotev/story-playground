using Backend.Models;
using Google.GenAI;

namespace Backend.Services;

public class GoogleAIService : ILLMService
{
    private readonly ILogger<GoogleAIService> _logger;
    private Settings _settings;
    public GoogleAIService(ILogger<GoogleAIService> logger, Settings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    public async Task<string> Generate(string prompt, CancellationToken cancellationToken)
    {
        var client = new Client(false, _settings.ApiKey);

        var response = await client.Models.GenerateContentAsync(_settings.LlmModel, prompt);
        return response.Candidates[0].Content.Parts[0].Text;
    }
}