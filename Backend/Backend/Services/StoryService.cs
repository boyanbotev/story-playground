
using Backend.Models.Db;
using Backend.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Services;

public enum AddResult
{
    Success,
    AlreadyExists,
    Invalid
}

public enum RemoveResult
{
    Success,
    DoesNotExist
}

public enum UpdateResult
{
    Success,
    DoesNotExist
}

public class StoryService : IStoryService
{
    public StoryContext db { get; }
    public ILLMService LLMService { get; }
    private IPromptService promptService;
    private Settings settings;
    public StoryService(StoryContext storyContext, ILLMService lLMService, IPromptService promptService, Settings settings)
    {
        db = storyContext;
        LLMService = lLMService;
        this.settings = settings;
        this.promptService = promptService;
    }

    public async Task<Story[]> GetStories()
    {
        return await db.Stories.Include(s => s.Nodes).AsNoTracking().ToArrayAsync();
    }

    public async Task<AddResult> AddStory(AddStoryRequest addRequest)
    {
        var story = new Story();
        story.Name = addRequest.Name;
        story.Structure = addRequest.Structure;

        story.Nodes = new List<StoryNode>();
        foreach (var node in addRequest.Nodes)
        {
            var storyNode = new StoryNode
            {
                Content = node.Content,
                Turns = node.Turns,
            };
            story.Nodes.Add(storyNode);
        }
        Console.WriteLine(story.Nodes.Count);

        await db.Stories.AddAsync(story);
         try {
            await db.SaveChangesAsync();
        } catch (DbUpdateException) {
            return AddResult.AlreadyExists;
        }
        return AddResult.Success;
    }
    public async Task<Story> GetStory(int id)
    {
        return await db.Stories.Include(s => s.Nodes).AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<UpdateResult> UpdateStory(int id, UpdateStoryRequest updateStoryRequest)
    {
        var story = await db.Stories.Include(s => s.Nodes).FirstOrDefaultAsync(s => s.Id == id);

        if (story == null)
        {
            return UpdateResult.DoesNotExist;
        }

        story.Name = updateStoryRequest.Name;
        story.Structure = updateStoryRequest.Structure;
        story.StartingSummary = updateStoryRequest.StartingSummary;
        story.Introduction = updateStoryRequest.Introduction;

        story.Nodes.Clear();
        foreach (var node in updateStoryRequest.Nodes)
        {
            var storyNode = new StoryNode
            {
                Content = node.Content,
                Turns = node.Turns,
            };
            story.Nodes.Add(storyNode);
        }

        await db.SaveChangesAsync();
        return UpdateResult.Success;
    }

    public async Task<RemoveResult> DeleteStory(int id)
    {
        var story = await db.Stories.FindAsync(id);

        if (story == null)
        {
            return RemoveResult.DoesNotExist;
        }

        db.Stories.Remove(story);
        await db.SaveChangesAsync();
        return RemoveResult.Success;
    }

    public async Task<object> ProgressStory(ProgressRequest progressRequest)
    {
        var story = await GetStory(progressRequest.StoryId);

        bool isValid = await ValidateUserAction(progressRequest.UserAction, story.Structure, progressRequest.SummarySoFar);
        if (!isValid)
        {
            var error = "Invalid User Action";
            var errorResponse = new
            {
                error,
            };
            return errorResponse;
        }

        var nextNode = story.Nodes[progressRequest.NodeIndex].Content;
        string prompt = CreateProgressPrompt(progressRequest, nextNode);

        Console.WriteLine("Prompt:\n" + prompt);
        string storyText = await LLMService.Generate(prompt);
        var response = await CreateProgressResponse(progressRequest, storyText);

        return response;
    }

    public async Task<bool> ValidateUserAction(string userAction, string storyStructure, string storySoFar)
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

    private string CreateProgressPrompt(ProgressRequest progressRequest, string nextNode)
    {
        var actionPrompt = promptService.Load("follow_user_action");
        var transitionTemplate = promptService.Load("transition_to_node");
        var experienceTemplate = promptService.Load("experience_node");
        var transitionPrompt = promptService.Fill(transitionTemplate, new Dictionary<string, string>
        {
            { "NextNode", nextNode },
        });
        var experiencePrompt = promptService.Fill(experienceTemplate, new Dictionary<string, string>
        {
            { "NextNode", nextNode },
        });

        string steeringInstruction = progressRequest.TurnsRemaining switch
        {
            0 => experiencePrompt,
            <= 2 => transitionPrompt,
            _ => actionPrompt
        };

        string upcomingEventContext = progressRequest.TurnsRemaining <= 2
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

    private async Task<object> CreateProgressResponse(ProgressRequest progressRequest, string storyText)
    {
        var turnsRemaining = progressRequest.TurnsRemaining - 1;
        var nodeIndex = progressRequest.NodeIndex;
        bool hasReachedNode = turnsRemaining == 0;

        if (hasReachedNode)
        {
            nodeIndex++;
            var story = await GetStory(progressRequest.StoryId);
            turnsRemaining = story.Nodes[nodeIndex].Turns;
        }

        var template = promptService.Load("summary");
        var prompt = promptService.Fill(template, new Dictionary<string, string>
        {
            { "StoryText", storyText },
            { "SummaryUnnecessaryPhrase", settings.SummaryUnnecessaryPhrase },
        });

        string summaryExtension = await LLMService.Generate(prompt);
        summaryExtension = summaryExtension.Contains(settings.SummaryUnnecessaryPhrase) ? "" : summaryExtension;
        string summarySoFar = progressRequest.SummarySoFar + " " + summaryExtension;
        var response = new
        {
            storyText,
            summarySoFar,
            nodeIndex,
            turnsRemaining,
        };
        return response;
    }
}