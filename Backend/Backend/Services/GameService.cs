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

    public async Task<ProgressResponse> ProgressStory(ProgressRequest progressRequest, string userId, CancellationToken cancellationToken)
    {
        var story = await storyService.GetStory(progressRequest.StoryId, userId, cancellationToken);

        var response = await validationService.ValidateUserAction(progressRequest, story, cancellationToken);

        if (response.result == ValidationResult.Invalid) return RejectUserAction();
        if (response.result == ValidationResult.Error) return RejectLLM(response.error);

        var progressResponse = await storyEngine.ProcessTurn(progressRequest, story, cancellationToken);

        if (progressResponse.Completed) progressResponse.SummarySoFar = progressRequest.SummarySoFar;
        else
        {
            var summaryResponse = await summaryService.GenerateSummary(progressRequest, progressResponse.StoryText, cancellationToken);
            if (summaryResponse.Error != null) return RejectLLM(summaryResponse.Error);
            progressResponse.SummarySoFar = summaryResponse.Text;
        }

        return progressResponse;
    }

    private static ProgressResponse RejectUserAction()
    {
        var errorResponse = new ProgressResponse();
        errorResponse.Error = "Invalid User Action";
        return errorResponse;
    }

    private static ProgressResponse RejectLLM(string error)
    {
        var errorResponse = new ProgressResponse();
        errorResponse.Error = error;
        return errorResponse;
    }
}