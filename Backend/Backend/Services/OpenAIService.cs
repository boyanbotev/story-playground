using OpenAI.Chat;
using Backend.Models;

namespace Backend.Services;

public class OpenAIService : ILLMService
{
    private readonly ILogger<OpenAIService> _logger;
    private Settings _settings;
    public OpenAIService(ILogger<OpenAIService> logger, Settings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    public async Task<string> Generate(string prompt, CancellationToken cancellationToken)
    {
        ChatClient client = new ("gpt-4o-mini", _settings.ApiKey);
        var response = await client.CompleteChatAsync(prompt);

        string text = response.Value.Content[0].Text;
        _logger.LogInformation(text);

        return text;
    }
}