using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

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

    public async Task<bool> ValidateUserAction(string userAction, string storyStructure, string storySoFar)
    {
        string prompt = $@"
        Story Structure: {storyStructure}
        Story So Far: {storySoFar}
        Judge if the following user action is physically possible for the character.
        Accept ANY speech by the main character even if it is implausible or ridiculous.
        Only reject actions if they are not physically plausible (ie. introducing a gun, or a character that doesn't exist yet).
        CRITICAL: reject the user action if it introduces characters of objects.
        User Action: {userAction}

        Answer exactly in this format:
        YES
        or
        NO
        ";
        string isTrue = await Generate(prompt);
        return isTrue.ToLower().Contains("yes");
    }
}