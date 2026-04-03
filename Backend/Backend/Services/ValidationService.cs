using Backend.Models.Db;
using Backend.Models.DTO;

namespace Backend.Services;

public class ValidationService : IValidationService
{
    public ILLMService LLMService { get; }
    private IPromptService promptService;
    private ILogger<ValidationService> logger;
    public ValidationService(ILLMService lLMService, IPromptService promptService, ILogger<ValidationService> logger)
    {
        LLMService = lLMService;
        this.promptService = promptService;
        this.logger = logger;
    }

    public async Task<bool> ValidateUserAction(ProgressRequest progressRequest, Story story, CancellationToken cancellationToken)
    {
        bool isPlausible = await ValidatePlausibility(progressRequest.UserAction, story.Structure, progressRequest.SummarySoFar, cancellationToken);
        bool isControllingCorrectCharacter = await ValidateCharacter(progressRequest.UserAction, story.MainCharacterName, cancellationToken);
        if (!isPlausible || !isControllingCorrectCharacter) return false;

        return true;
    }

    private async Task<bool> ValidatePlausibility(string userAction, string storyStructure, string storySoFar, CancellationToken cancellationToken)
    {
        var template = promptService.Load("validate_action_plausibility");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "StoryStructure", storyStructure },
            { "StorySoFar", storySoFar },
            { "UserAction", userAction },
        });

        return await Validate(prompt, cancellationToken);
    }

    private async Task<bool> ValidateCharacter(string userAction, string mainCharacter, CancellationToken cancellationToken)
    {
        var template = promptService.Load("validate_action_character");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "MainCharacter", mainCharacter },
            { "UserAction", userAction },
        });

        return await Validate(prompt, cancellationToken);
    }

    public async Task<bool> ValidateGoalReached(string textToCheck, string characterGoal, string storySoFar, CancellationToken cancellationToken)
    {
        var template = promptService.Load("validate_goal_reached");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "TextToCheck", textToCheck },
            { "CharacterGoal", characterGoal },
            { "StorySoFar", storySoFar },
        });

        return await Validate(prompt, cancellationToken);
    }

    public async Task<bool> Validate(string prompt, CancellationToken cancellationToken)
    {
        string isTrue = await LLMService.Generate(prompt, cancellationToken);
        
        var normalized = isTrue.Trim().ToUpper();

        if (normalized == "YES") return true;
        if (normalized == "NO") return false;

        logger.LogWarning($"Invalid response from LLM: {isTrue}");
        return false;
    }
}