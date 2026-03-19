
using server.models.db;
using server.models.DTO;
using Microsoft.EntityFrameworkCore;
using server.models;

namespace server.Services;

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
        return await db.Stories.AsNoTracking().ToArrayAsync();
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
        return await db.Stories.FindAsync(id);
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
        bool isValid = await ValidateUserAction(progressRequest.UserAction, progressRequest.StoryStructure, progressRequest.SummarySoFar);
        if (!isValid)
        {
            return "Invalid User Action";
        }

        string prompt = CreateProgressPrompt(progressRequest);

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

    private string CreateProgressPrompt(ProgressRequest progressRequest)
    {
        string steeringInstruction = progressRequest.TurnsRemaining switch
        {
            0 => $"CRITICAL INSTRUCTION: The user MUST now experience this exact event: '{progressRequest.NextNode}'. Make it happen in this response.",

            <= 2 => $"CRITICAL INSTRUCTION: Transition the scene to set up this upcoming event: '{progressRequest.NextNode}'. DO NOT let the event actually happen yet. Just put the pieces in place based on the User Action.",

            _ => $"CRITICAL INSTRUCTION: Focus ENTIRELY on the User Action. Do NOT introduce any new characters or major events yet."
        };

        string upcomingEventContext = progressRequest.TurnsRemaining <= 2
            ? $"\nUpcoming Event to foreshadow: {progressRequest.NextNode}"
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
        var turnsRemaining = progressRequest.TurnsRemaining - 1; // TODO: we in fact should calculate this from the next node object

        string summaryExtension = await LLMService.Generate($"Write a very brief condensed summary of the following:\n{storyText}. ONLY include the summary text. 5-10 words. ONLY THE FACTS. Keep it as short and accurate as possible. If nothing highly significant happened answer NOTHING_MUCH_HAPPENED.");
        summaryExtension = summaryExtension.Contains("NOTHING_MUCH_HAPPENED") ? "" : summaryExtension;
        string summarySoFar = progressRequest.SummarySoFar + " " + summaryExtension;
        var response = new
        {
            storyText,
            summarySoFar,
            turnsRemaining,
        };
        return response;
    }
}