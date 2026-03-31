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

    public async Task<ProgressResponse> ProgressStory(ProgressRequest progressRequest, CancellationToken cancellationToken)
    {
        var story = await storyService.GetStory(progressRequest.StoryId, cancellationToken);

        if (!await validationService.ValidateUserAction(progressRequest, story, cancellationToken)) return RejectUserAction();

        var progressResponse = await storyEngine.ProcessTurn(progressRequest, story, cancellationToken);

        progressResponse.SummarySoFar = progressResponse.Completed 
            ? progressRequest.SummarySoFar 
            : await summaryService.GenerateSummary(progressRequest, progressResponse.StoryText, cancellationToken);

        return progressResponse;
    }

    private static ProgressResponse RejectUserAction()
    {
        var errorResponse = new ProgressResponse();
        errorResponse.Error = "Invalid User Action";
        return errorResponse;
    }
}