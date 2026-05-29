using Backend.Models.Db;
using Backend.Models.DTO;

namespace Backend.Services;
public class StoryEngine : IStoryEngine
{
    IPromptBuilder promptBuilder;
    ILLMService LLMService;
    IValidationService validationService;
    public StoryEngine(IPromptBuilder promptBuilder, ILLMService lLMService, IValidationService validationService)
    {
        this.promptBuilder = promptBuilder;
        this.validationService = validationService;
        this.LLMService = lLMService;
    }

    public async Task<ProgressResponse> ProcessTurn(ProgressRequest progressRequest, Story story, CancellationToken cancellationToken)
    {
        var node = story.Nodes[progressRequest.NodeIndex];

        string storyText;
        ProgressResponse status;

        if (node is StoryNode)
        {
            string prompt = promptBuilder.CreateStoryPrompt(progressRequest, node as StoryNode);
            var response = await LLMService.Generate(prompt, cancellationToken);
            storyText = response.Text;

            if (response.Error != null) return LLMError(response.Error);

            status = GetStatus(progressRequest, storyText, story);
        }
        else
        {
            QuestNode questNode = node as QuestNode;
            string prompt = promptBuilder.CreateQuestPrompt(progressRequest, questNode);
            var response = await LLMService.Generate(prompt, cancellationToken);
            storyText = response.Text;

            if (response.Error != null) return LLMError(response.Error);

            status = await GetQuestStatus(progressRequest, story, storyText, questNode, cancellationToken);
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

    private async Task<ProgressResponse> GetQuestStatus(ProgressRequest progressRequest, Story story, string storyText, QuestNode questNode, CancellationToken cancellationToken)
    {
        ValidationResponse isGoalReached = await validationService.ValidateGoalReached(storyText, questNode.UserGoal, progressRequest.SummarySoFar, cancellationToken);
        if (isGoalReached.result.Equals(ValidationResult.Success))
        {
            var nextNodeStatus = GetNextNodeResponse(progressRequest, story);
            nextNodeStatus.QuestCompleteText = $"QUEST COMPLETE: {questNode.UserGoal}";
            return nextNodeStatus;
        }
        else if (isGoalReached.result.Equals(ValidationResult.Error))
        {
            return LLMError(isGoalReached.error);
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

    ProgressResponse LLMError(string error)
    {
        var status = new ProgressResponse();
        status.Error = error;
        return status;
    }
}