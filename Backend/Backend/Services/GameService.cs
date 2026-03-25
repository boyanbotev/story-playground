
using Backend.Models.Db;
using Backend.Models.DTO;
using Backend.Models;

namespace Backend.Services;

public class StoryStatus
{
    public int NodeIndex { get; set; }
    public string Content { get; set; }
    public int? TransitionTurnsRemaining { get; set; }
    public int? ContentTurnsRemaining { get; set; }
    public string? UserGoal { get; set; }
    public string? Difficulty { get; set; }

}

public class GameService : IGameService
{
    public ILLMService LLMService { get; }
    private IPromptService promptService;
    private Settings settings;
    private IStoryService storyService;
    public GameService(ILLMService lLMService, IPromptService promptService, Settings settings, IStoryService storyService)
    {
        LLMService = lLMService;
        this.settings = settings;
        this.promptService = promptService;
        this.storyService = storyService;
    }

    public async Task<object> ProgressStory(ProgressRequest progressRequest)
    {
        var story = await storyService.GetStory(progressRequest.StoryId);

        bool isValid = await ValidateUserAction(progressRequest.UserAction, story.Structure, progressRequest.SummarySoFar);
        if (!isValid) return RejectUserAction();

        var node = story.Nodes[progressRequest.NodeIndex];

        string storyText;
        StoryStatus status;

        if (node is StoryNode)
        {
            string prompt = CreateStoryPrompt(progressRequest, node as StoryNode);
            storyText = await LLMService.Generate(prompt);
            status = GetStatus(progressRequest, storyText, story);
        }
        else
        {
            QuestNode questNode = node as QuestNode;
            string prompt = CreateQuestPrompt(progressRequest, questNode);
            storyText = await LLMService.Generate(prompt);
            status = await GetQuestStats(progressRequest, story, storyText, questNode);
        }

        var summarySoFar = await GenerateSummary(progressRequest, storyText);

        return new
        {
            storyText,
            summarySoFar,
            status.NodeIndex,
            status.TransitionTurnsRemaining,
            status.ContentTurnsRemaining,
            status.UserGoal,
            status.Difficulty,
        };
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

    private async Task<bool> ValidateUserAction(string userAction, string storyStructure, string storySoFar)
    {
        var template = promptService.Load("validate");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "StoryStructure", storyStructure },
            { "StorySoFar", storySoFar },
            { "UserAction", userAction },
        });

        string isTrue = await LLMService.Generate(prompt);
        return isTrue.ToLower().Contains("yes");
    }

    private async Task<bool> IsGoalReached(string textToCheck, string characterGoal, string storySoFar)
    {
        var template = promptService.Load("validate_goal_reached");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "TextToCheck", textToCheck },
            { "CharacterGoal", characterGoal },
            { "StorySoFar", storySoFar },
        });

        string isTrue = await LLMService.Generate(prompt);
        return isTrue.ToLower().Contains("yes");
    }

    private string CreateQuestPrompt(ProgressRequest progressRequest, QuestNode questNode)
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

    private string CreateStoryPrompt(ProgressRequest progressRequest, StoryNode storyNode)
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

    private StoryStatus GetNextNodeResponse(ProgressRequest progressRequest, Story story)
    {
        var nodeIndex = progressRequest.NodeIndex + 1;
        var nextNode = story.Nodes[nodeIndex];

        if (nextNode is StoryNode)
        {
            var storyNode = nextNode as StoryNode;
            var status = new StoryStatus();
            status.Content = storyNode.Content;
            status.TransitionTurnsRemaining = storyNode.TransitionTurns;
            status.ContentTurnsRemaining = storyNode.ContentTurns;
            status.NodeIndex = nodeIndex;
            return status;
        }
        else
        {
            var questNode = nextNode as QuestNode;
            var status = new StoryStatus();
            status.UserGoal = questNode.UserGoal;
            status.Difficulty = questNode.Difficulty;
            status.Content = questNode.Content;
            status.NodeIndex = nodeIndex;
            return status;
        }
    }

    private async Task<StoryStatus> GetQuestStats(ProgressRequest progressRequest, Story story, string storyText, QuestNode questNode)
    {
        bool isGoalReached = await IsGoalReached(storyText, questNode.UserGoal, progressRequest.SummarySoFar);
        if (isGoalReached)
        {
            return GetNextNodeResponse(progressRequest, story);
        }

        var status = new StoryStatus();
        status.NodeIndex = progressRequest.NodeIndex;
        status.Difficulty = (story.Nodes[progressRequest.NodeIndex] as QuestNode).Difficulty;
        status.UserGoal = (story.Nodes[progressRequest.NodeIndex] as QuestNode).UserGoal;
        return status;
    }

    private StoryStatus GetStatus(ProgressRequest progressRequest, string storyText, Story story)
    {
        var nodeIndex = progressRequest.NodeIndex;
        var transitionTurnsRemaining = progressRequest.TransitionTurnsRemaining;
        var contentTurnsRemaining = progressRequest.ContentTurnsRemaining;

        if (transitionTurnsRemaining > 0) transitionTurnsRemaining--;
        else contentTurnsRemaining--;
        
        if (transitionTurnsRemaining < 1 && contentTurnsRemaining < 1)
        {
            return GetNextNodeResponse(progressRequest, story);
        }

        StoryStatus status = new StoryStatus();
        status.NodeIndex = nodeIndex;
        status.TransitionTurnsRemaining = transitionTurnsRemaining;
        status.ContentTurnsRemaining = contentTurnsRemaining;

        return status;
    }

    private async Task<string> GenerateSummary(ProgressRequest progressRequest, string storyText)
    {
        var template = promptService.Load("summary");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "StoryText", storyText },
            { "SummaryUnnecessaryPhrase", settings.SummaryUnnecessaryPhrase },
        });

        string summaryExtension = await LLMService.Generate(prompt);
        summaryExtension = summaryExtension.Contains(settings.SummaryUnnecessaryPhrase) ? "" : summaryExtension;
        return progressRequest.SummarySoFar + " " + summaryExtension;
    }
}