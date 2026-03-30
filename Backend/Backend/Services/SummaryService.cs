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

    public async Task<string> GenerateSummary(ProgressRequest progressRequest, string storyText, CancellationToken cancellationToken)
    {
        var template = promptService.Load("summary");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "StoryText", storyText },
            { "SummaryUnnecessaryPhrase", settings.SummaryUnnecessaryPhrase },
        });

        string summaryExtension = await LLMService.Generate(prompt, cancellationToken);
        summaryExtension = summaryExtension.Contains(settings.SummaryUnnecessaryPhrase) ? "" : summaryExtension;
        return progressRequest.SummarySoFar + " " + summaryExtension;
    }
}