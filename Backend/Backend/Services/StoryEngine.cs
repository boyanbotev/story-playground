using Backend.Models.Db;
using Backend.Models.DTO;

namespace Backend.Services;
public class StoryEngine : IStoryEngine
{
    IPromptBuilder promptBuilder;
    ILLMService LLMService;
    IPromptService promptService;
    public StoryEngine(IPromptBuilder promptBuilder, ILLMService lLMService, IPromptService promptService)
    {
        this.promptBuilder = promptBuilder;
        this.LLMService = lLMService;
        this.promptService = promptService;
    }

    public async Task<ProgressResponse> ProcessTurn(ProgressRequest progressRequest, Story story)
    {
        var node = story.Nodes[progressRequest.NodeIndex];

        string storyText;
        ProgressResponse status;

        if (node is StoryNode)
        {
            string prompt = promptBuilder.CreateStoryPrompt(progressRequest, node as StoryNode);
            storyText = await LLMService.Generate(prompt);
            status = GetStatus(progressRequest, storyText, story);
        }
        else
        {
            QuestNode questNode = node as QuestNode;
            string prompt = promptBuilder.CreateQuestPrompt(progressRequest, questNode);
            storyText = await LLMService.Generate(prompt);
            status = await GetQuestStatus(progressRequest, story, storyText, questNode);
        }
        status.StoryText = storyText;

        return status;
    }
    
    private ProgressResponse GetNextNodeResponse(ProgressRequest progressRequest, Story story)
    {
        var status = new ProgressResponse();
        var nodeIndex = progressRequest.NodeIndex + 1;
        if (nodeIndex >= story.Nodes.Count)
        {
            status.Completed = true;
            return status;
        }

        var nextNode = story.Nodes[nodeIndex];

        if (nextNode is StoryNode)
        {
            var storyNode = nextNode as StoryNode;
            status.TransitionTurnsRemaining = storyNode.TransitionTurns;
            status.ContentTurnsRemaining = storyNode.ContentTurns;
            status.NodeIndex = nodeIndex;
            return status;
        }
        else
        {
            var questNode = nextNode as QuestNode;
            status.UserGoal = questNode.UserGoal;
            status.Difficulty = questNode.Difficulty;
            status.NodeIndex = nodeIndex;
            return status;
        }
    }

    public async Task<bool> IsGoalReached(string textToCheck, string characterGoal, string storySoFar)
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

    private async Task<ProgressResponse> GetQuestStatus(ProgressRequest progressRequest, Story story, string storyText, QuestNode questNode)
    {
        bool isGoalReached = await IsGoalReached(storyText, questNode.UserGoal, progressRequest.SummarySoFar);
        if (isGoalReached)
        {
            var nextNodeStatus = GetNextNodeResponse(progressRequest, story);
            nextNodeStatus.QuestCompleteText = $"QUEST COMPLETE: {questNode.UserGoal}";
            return nextNodeStatus;
        }

        var status = new ProgressResponse();
        status.NodeIndex = progressRequest.NodeIndex;
        status.Difficulty = (story.Nodes[progressRequest.NodeIndex] as QuestNode).Difficulty;
        status.UserGoal = (story.Nodes[progressRequest.NodeIndex] as QuestNode).UserGoal;
        return status;
    }

    private ProgressResponse GetStatus(ProgressRequest progressRequest, string storyText, Story story)
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

        ProgressResponse status = new();
        status.NodeIndex = nodeIndex;
        status.TransitionTurnsRemaining = transitionTurnsRemaining;
        status.ContentTurnsRemaining = contentTurnsRemaining;

        return status;
    }
}