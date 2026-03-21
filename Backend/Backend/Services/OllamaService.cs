using System.Text;
using Newtonsoft.Json;

namespace Backend.Services;
public class OllamaService : ILLMService
{
    private readonly HttpClient _client;
    public OllamaService(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> Generate(string prompt)
    {
        var request = new
        {
            model = "gemma3:12b",
            prompt,
            stream = false,
        };

        var json = System.Text.Json.JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("http://localhost:11434/api/generate", content);
        var result = await response.Content.ReadAsStringAsync();

        dynamic obj = JsonConvert.DeserializeObject(result);
        string text = obj.response;

        Console.WriteLine(text);

        return text;
    }
}