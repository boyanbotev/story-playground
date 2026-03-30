using Backend.Models.Db;
using Backend.Models.DTO;

namespace Backend.Services;

public class ValidationService : IValidationService
{
    public ILLMService LLMService { get; }
    private IPromptService promptService;
    public ValidationService(ILLMService lLMService, IPromptService promptService)
    {
        LLMService = lLMService;
        this.promptService = promptService;
    }

    public async Task<bool> ValidateUserAction(ProgressRequest progressRequest, Story story)
    {
        bool isPlausible = await ValidatePlausibility(progressRequest.UserAction, story.Structure, progressRequest.SummarySoFar);
        bool isControllingCorrectCharacter = await ValidateCharacter(progressRequest.UserAction, story.MainCharacterName);
        if (!isPlausible || !isControllingCorrectCharacter) return false;

        return true;
    }

    private async Task<bool> ValidatePlausibility(string userAction, string storyStructure, string storySoFar)
    {
        var template = promptService.Load("validate_action_plausibility");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "StoryStructure", storyStructure },
            { "StorySoFar", storySoFar },
            { "UserAction", userAction },
        });

        string isTrue = await LLMService.Generate(prompt);
        return isTrue.ToLower().Contains("yes");
    }

    private async Task<bool> ValidateCharacter(string userAction, string mainCharacter)
    {
        var template = promptService.Load("validate_action_character");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "MainCharacter", mainCharacter },
            { "UserAction", userAction },
        });

        string isTrue = await LLMService.Generate(prompt);
        return isTrue.ToLower().Contains("yes");
    }
}