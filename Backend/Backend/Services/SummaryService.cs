using Backend.Models.DTO;
using Backend.Models;

namespace Backend.Services;

public class SummaryService : ISummaryService
{
    public ILLMService LLMService { get; }
    private IPromptService promptService;
    private Settings settings;
    public SummaryService(ILLMService lLMService, IPromptService promptService, Settings settings)
    {
        LLMService = lLMService;
        this.promptService = promptService;
        this.settings = settings;
    }

    public async Task<LLMResponse> GenerateSummary(ProgressRequest progressRequest, string storyText, string apiKey, CancellationToken cancellationToken)
    {
        var template = promptService.Load("summary");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "StoryText", storyText },
            { "SummaryUnnecessaryPhrase", settings.SummaryUnnecessaryPhrase },
        });

        var response = await LLMService.Generate(prompt, apiKey, cancellationToken);
        string summaryExtension = response.Text;

        summaryExtension = summaryExtension.Contains(settings.SummaryUnnecessaryPhrase) ? "" : summaryExtension;
        response.Text = progressRequest.SummarySoFar + " " + summaryExtension;
        return response;
    }
}