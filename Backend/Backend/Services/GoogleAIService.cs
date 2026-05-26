using Backend.Models;
using Google.GenAI;
using Backend.Models.DTO;

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

    public async Task<LLMResponse> Generate(string prompt, string apiKey, CancellationToken cancellationToken)
    {
        var client = new Client(false, apiKey);

        try
        {
            var response = await client.Models.GenerateContentAsync(_settings.LlmModel, prompt);
            string text = response.Candidates[0].Content.Parts[0].Text;
            return new LLMResponse(text, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new LLMResponse(null, ex.Message);
        }
    }
}