
namespace Backend.Models;
public class Settings
{
    public string SystemPrompt { get; set; }
    public string StyleGuide { get; set; }
    public string ForeshadowText { get; set; }
    public string SummaryUnnecessaryPhrase { get; set; }
    public string LlmBaseUrl { get; set; }
    public string LlmModel { get; set; }
    public string BearerKey { get; set; }
    public string JwtIssuer { get; set; }
    public string JwtAudience { get; set; }
}