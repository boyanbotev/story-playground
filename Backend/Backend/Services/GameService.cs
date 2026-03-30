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

        var result = await storyEngine.ProcessTurn(progressRequest, story);
        
        if (result.Completed) {
                return new {
                completed = true,
                storyText = result.StoryText,
            };
        }

        var summarySoFar = await summaryService.GenerateSummary(progressRequest, result.StoryText);

        return MapResponse(result, summarySoFar);
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

    public object MapResponse(StoryStatus status, string summarySoFar)
    {
        return new
        {
            status.StoryText,
            summarySoFar,
            status.NodeIndex,
            status.TransitionTurnsRemaining,
            status.ContentTurnsRemaining,
            status.UserGoal,
            status.Difficulty,
            status.QuestCompleteText,
        };
    }
}