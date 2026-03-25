
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
    public StoryService(StoryContext storyContext, ILLMService lLMService, IPromptService promptService, Settings settings)
    {
        db = storyContext;
        LLMService = lLMService;
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
        story.StartingSummary = addRequest.StartingSummary;
        story.Introduction = addRequest.Introduction;

        story.Nodes = new List<Node>();
        foreach (var node in addRequest.Nodes)
        {
            switch (node)
            {
                case StoryNodeRequest storyNode:
                    story.Nodes.Add(new StoryNode
                    {
                        Content = storyNode.Content,
                        TransitionTurns = storyNode.TransitionTurns,
                        ContentTurns = storyNode.ContentTurns,
                    });
                    break;
                case QuestNodeRequest questNode:
                    story.Nodes.Add(new QuestNode
                    {
                        Content = questNode.Content,
                        UserGoal = questNode.UserGoal,
                        Difficulty = questNode.Difficulty,
                    });
                    break;
            }
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
            switch (node)
            {
                case StoryNodeRequest storyNode:
                    story.Nodes.Add(new StoryNode
                    {
                        Content = storyNode.Content,
                        TransitionTurns = storyNode.TransitionTurns,
                        ContentTurns = storyNode.ContentTurns,
                    });
                    break;
                case QuestNodeRequest questNode:
                    story.Nodes.Add(new QuestNode
                    {
                        Content = questNode.Content,
                        UserGoal = questNode.UserGoal,
                        Difficulty = questNode.Difficulty,
                    });
                    break;
            }
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
}
