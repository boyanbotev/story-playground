
using Backend.Models.Db;
using Backend.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public enum AddResult
{
    Success,
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
    private readonly StoryContext db;
    private ILogger<StoryService> logger;
    public StoryService(StoryContext storyContext, ILogger<StoryService> logger)
    {
        db = storyContext;
        this.logger = logger;
    }

    public async Task<Story[]> GetStories(string userId, CancellationToken cancellationToken)
    {
        return await db.Stories.AsNoTracking()
            .Where(s => s.UserId == userId)
            .Include(s => s.Nodes.OrderBy(n => n.Order))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<AddResult> AddStory(AddStoryRequest addRequest, string userId, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken) as Backend.Models.Db.User;

        var story = new Story();
        story.Name = addRequest.Name;
        story.Structure = addRequest.Structure;
        story.StartingSummary = addRequest.StartingSummary;
        story.Introduction = addRequest.Introduction;
        story.MainCharacterName = addRequest.MainCharacterName;
        story.User = user;

        story.Nodes = new List<Node>();
        int order = 0;
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
                        Order = order++,
                    });
                    break;
                case QuestNodeRequest questNode:
                    story.Nodes.Add(new QuestNode
                    {
                        UserGoal = questNode.UserGoal,
                        Difficulty = questNode.Difficulty,
                        Order = order++,
                    });
                    break;
                default:
                    throw new ArgumentException("Invalid node type");
            }
        }

        await db.Stories.AddAsync(story, cancellationToken);
         try {
            await db.SaveChangesAsync(cancellationToken);
        } catch (DbUpdateException e) {
            logger.LogError(e.Message);
            return AddResult.Invalid;
        }
        return AddResult.Success;
    }
    public async Task<Story> GetStory(int id, string userId, CancellationToken cancellationToken)
    {
        return await db.Stories.Include(s => s.Nodes.OrderBy(n => n.Order)).AsNoTracking().FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId, cancellationToken);
    }

    public async Task<UpdateResult> UpdateStory(int id, string userId, UpdateStoryRequest updateStoryRequest, CancellationToken cancellationToken)
    {
        var story = await db.Stories.Include(s => s.Nodes.OrderBy(n => n.Order)).FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId, cancellationToken);

        if (story == null)
        {
            return UpdateResult.DoesNotExist;
        }

        story.Name = updateStoryRequest.Name;
        story.Structure = updateStoryRequest.Structure;
        story.StartingSummary = updateStoryRequest.StartingSummary;
        story.Introduction = updateStoryRequest.Introduction;
        story.MainCharacterName = updateStoryRequest.MainCharacterName;

        story.Nodes.RemoveRange(0, story.Nodes.Count);
        int order = 0;
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
                        Order = order++,
                    });
                    break;
                case QuestNodeRequest questNode:
                    story.Nodes.Add(new QuestNode
                    {
                        UserGoal = questNode.UserGoal,
                        Difficulty = questNode.Difficulty,
                        Order = order++,
                    });
                    break;
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return UpdateResult.Success;
    }

    public async Task<RemoveResult> DeleteStory(int id, string userId, CancellationToken cancellationToken)
    {
        var story = await db.Stories.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId, cancellationToken);

        if (story == null)
        {
            return RemoveResult.DoesNotExist;
        }

        db.Stories.Remove(story);
        await db.SaveChangesAsync(cancellationToken);
        return RemoveResult.Success;
    }
}
