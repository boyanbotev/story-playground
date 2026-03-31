using System.Text;
using Newtonsoft.Json;
using Backend.Models;

namespace Backend.Services;
public class OllamaService : ILLMService
{
    private readonly HttpClient _client;
    private readonly ILogger<OllamaService> _logger;
    private Settings _settings;
    public OllamaService(HttpClient client, ILogger<OllamaService> logger, Settings settings)
    {
        _client = client;
        _logger = logger;
        _settings = settings;
    }

    public async Task<string> Generate(string prompt, CancellationToken cancellationToken)
    {
        var request = new
        {
            model = _settings.LlmModel,
            prompt,
            stream = false,
        };

        var json = System.Text.Json.JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"{_settings.LlmBaseUrl}/api/generate", content, cancellationToken);
        var result = await response.Content.ReadAsStringAsync(cancellationToken);

        dynamic obj = JsonConvert.DeserializeObject(result);
        string text = obj.response;

        _logger.LogInformation(text);

        return text;
    }
}