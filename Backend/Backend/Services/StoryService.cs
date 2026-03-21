
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
    private Settings settings;
    public StoryService(StoryContext storyContext, ILLMService lLMService, Settings settings)
    {
        db = storyContext;
        LLMService = lLMService;
        this.settings = settings;
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
        string prompt = $@"
        Story Structure: {storyStructure}
        Story So Far: {storySoFar}
        Judge if the following user action is physically possible for the character.
        Accept ANY speech by the main character even if it is implausible or ridiculous.
        Only reject actions if they are not physically plausible (ie. introducing a gun, or a character that doesn't exist yet).
        CRITICAL: reject the user action if it introduces characters or objects.
        User Action: {userAction}

        Answer without reasoning exactly in this format:
        YES
        or
        NO
        ";
        string isTrue = await LLMService.Generate(prompt);
        return isTrue.ToLower().Contains("yes");
    }

    private string CreateProgressPrompt(ProgressRequest progressRequest, string nextNode)
    {
        string steeringInstruction = progressRequest.TurnsRemaining switch
        {
            0 => $"CRITICAL INSTRUCTION: The user MUST now experience this exact event: '{nextNode}'. Make it happen in this response.",

            <= 2 => $"CRITICAL INSTRUCTION: Transition the scene to set up this upcoming event: '{nextNode}'. DO NOT let the event actually happen yet. Just put the pieces in place based on the User Action.",

            _ => $"CRITICAL INSTRUCTION: Focus ENTIRELY on the User Action. Do NOT introduce any new characters or major events yet."
        };

        string upcomingEventContext = progressRequest.TurnsRemaining <= 2
            ? $"\nUpcoming Event to foreshadow: {nextNode}"
            : "";

        string prompt = $@"{settings.SystemPrompt}
Story Style Guide: {settings.StyleGuide}

Summary of story so far: {progressRequest.SummarySoFar}
{upcomingEventContext}

=== CURRENT TURN ===
User Action: {progressRequest.UserAction}
{steeringInstruction}

Write the next 40-70 words now:";
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

        string summaryExtension = await LLMService.Generate($"Write a very brief condensed summary of the following:\n{storyText}. ONLY include the summary text. 5-10 words. ONLY THE FACTS. Keep it as short and accurate as possible. If nothing highly significant happened answer NOTHING_MUCH_HAPPENED.");
        summaryExtension = summaryExtension.Contains("NOTHING_MUCH_HAPPENED") ? "" : summaryExtension;
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