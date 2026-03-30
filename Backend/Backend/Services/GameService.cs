using Backend.Models.DTO;

namespace Backend.Services;

public class GameService : IGameService
{
    public ILLMService LLMService { get; }
    private IStoryService storyService;
    private IValidationService validationService;
    private ISummaryService summaryService;
    private IStoryEngine storyEngine;
    public GameService(ILLMService lLMService, IStoryService storyService, IValidationService validationService, ISummaryService summaryService, IStoryEngine storyEngine)
    {
        LLMService = lLMService;
        this.storyService = storyService;
        this.validationService = validationService;
        this.summaryService = summaryService;
        this.storyEngine = storyEngine;
    }

    public async Task<object> ProgressStory(ProgressRequest progressRequest)
    {
        var story = await storyService.GetStory(progressRequest.StoryId);

        if (!await validationService.ValidateUserAction(progressRequest, story)) return RejectUserAction();

        var progressResponse = await storyEngine.ProcessTurn(progressRequest, story);

        progressResponse.SummarySoFar = progressResponse.Completed 
            ? progressRequest.SummarySoFar 
            : await summaryService.GenerateSummary(progressRequest, progressResponse.StoryText);

        return progressResponse;
    }

    private static object RejectUserAction()
    {
        var error = "Invalid User Action";
        var errorResponse = new
        {
            error,
        };
        return errorResponse;
    }
}