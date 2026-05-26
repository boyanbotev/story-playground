using Backend.Models.Db;
using Backend.Models.DTO;

namespace Backend.Services;

public enum ValidationResult
{
    Success,
    Invalid,
    Error
}

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

    public async Task<ValidationResponse> ValidateUserAction(ProgressRequest progressRequest, Story story, string apiKey, CancellationToken cancellationToken)
    {
        ValidationResponse isPlausible = await ValidatePlausibility(progressRequest.UserAction, story.Structure, progressRequest.SummarySoFar, apiKey, cancellationToken);
        ValidationResponse isControllingCorrectCharacter = await ValidateCharacter(progressRequest.UserAction, story.MainCharacterName, apiKey, cancellationToken);

        if (isPlausible.result == ValidationResult.Invalid || isControllingCorrectCharacter.result == ValidationResult.Invalid) return new ValidationResponse { result = ValidationResult.Invalid };
        if (isPlausible.result == ValidationResult.Error) return isPlausible;
        if (isControllingCorrectCharacter.result == ValidationResult.Error) return isControllingCorrectCharacter;

        return new ValidationResponse { result = ValidationResult.Success };
    }

    private async Task<ValidationResponse> ValidatePlausibility(string userAction, string storyStructure, string storySoFar, string apiKey, CancellationToken cancellationToken)
    {
        var template = promptService.Load("validate_action_plausibility");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "StoryStructure", storyStructure },
            { "StorySoFar", storySoFar },
            { "UserAction", userAction },
        });

        return await Validate(prompt, apiKey, cancellationToken);
    }

    private async Task<ValidationResponse> ValidateCharacter(string userAction, string mainCharacter, string apiKey, CancellationToken cancellationToken)
    {
        var template = promptService.Load("validate_action_character");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "MainCharacter", mainCharacter },
            { "UserAction", userAction },
        });

        return await Validate(prompt, apiKey, cancellationToken);
    }

    public async Task<ValidationResponse> ValidateGoalReached(string textToCheck, string characterGoal, string storySoFar, string apiKey, CancellationToken cancellationToken)
    {
        var template = promptService.Load("validate_goal_reached");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "TextToCheck", textToCheck },
            { "CharacterGoal", characterGoal },
            { "StorySoFar", storySoFar },
        });

        return await Validate(prompt, apiKey, cancellationToken);
    }

    public async Task<ValidationResponse> Validate(string prompt, string apiKey, CancellationToken cancellationToken)
    {
        ValidationResponse validationResponse = new();
        var response = await LLMService.Generate(prompt, apiKey, cancellationToken);

        if (response.Error != null)
        {
            logger.LogWarning($"Error generating LLM response");

            validationResponse.result = ValidationResult.Error;
            validationResponse.error = response.Error;
            return validationResponse;
        }

        string isTrue = response.Text;
        
        var normalized = isTrue.Trim().ToUpper();

        ValidationResult result;
        if (normalized == "YES") result = ValidationResult.Success;
        else if (normalized == "NO") result = ValidationResult.Invalid;
        else
        {
            logger.LogWarning($"Invalid response from LLM: {isTrue}");
            result = ValidationResult.Invalid;
        }

        return new ValidationResponse
        {
            result = result,
            error = null
        };
    }
}