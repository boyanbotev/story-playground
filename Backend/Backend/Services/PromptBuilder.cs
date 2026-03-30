using Backend.Models.DTO;
using Backend.Models.Db;
using Backend.Models;

namespace Backend.Services;

public class PromptBuilder : IPromptBuilder
{
    private IPromptService promptService;
    private Settings settings;
    public PromptBuilder(IPromptService promptService, Settings settings)
    {
        this.promptService = promptService;
        this.settings = settings;
    }
    
    public string CreateQuestPrompt(ProgressRequest progressRequest, QuestNode questNode)
    {
        var template = promptService.Load("quest");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "SystemPrompt", settings.SystemPrompt },
            { "StyleGuide", settings.StyleGuide },
            { "Difficulty", questNode.Difficulty },
            { "SummarySoFar", progressRequest.SummarySoFar},
            { "UserAction", progressRequest.UserAction },
        });

        return prompt;
    }

    public string CreateStoryPrompt(ProgressRequest progressRequest, StoryNode storyNode)
    {
        var nextNode = storyNode.Content;
        var actionPrompt = promptService.Load("follow_user_action");
        var transitionTemplate = promptService.Load("transition_to_node");
        var experienceTemplate = promptService.Load("experience_node");
        var continueTemplate = promptService.Load("continue_node");
        var transitionPrompt = promptService.Fill(transitionTemplate, new Dictionary<string, string>
        {
            { "NextNode", nextNode },
        });
        var experiencePrompt = promptService.Fill(experienceTemplate, new Dictionary<string, string>
        {
            { "NextNode", nextNode },
        });
        var continuePrompt = promptService.Fill(continueTemplate, new Dictionary<string, string>
        {
            { "NextNode", nextNode },
        });

        bool isContinueTurn = progressRequest.ContentTurnsRemaining < storyNode.ContentTurns;
        string steeringInstruction = progressRequest.TransitionTurnsRemaining switch
        {
            0 => isContinueTurn ? continuePrompt : experiencePrompt,
            <= 2 => transitionPrompt,
            _ => actionPrompt
        };

        string upcomingEventContext = progressRequest.TransitionTurnsRemaining <= 2
            ? $"\n{settings.ForeshadowText}{nextNode}"
            : "";

        var template = promptService.Load("progress");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "SystemPrompt", settings.SystemPrompt },
            { "StyleGuide", settings.StyleGuide },
            { "SummarySoFar", progressRequest.SummarySoFar },
            { "UpcomingEventContext", upcomingEventContext },
            { "UserAction", progressRequest.UserAction },
            { "SteeringInstruction", steeringInstruction },
        });

        return prompt;
    }
}