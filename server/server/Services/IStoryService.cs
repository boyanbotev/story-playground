using server.models.DTO;
using server.Services;
using server.models.db;

public interface IStoryService
{
    Task<AddResult> AddStory(AddStoryRequest addStoryRequest);
    Task<Story> GetStory(int id);
    Task<RemoveResult> DeleteStory(int id);
    Task<Story[]> GetStories();
    Task<bool> ValidateUserAction(string userAction, string storyStructure, string storySoFar);
    Task<object> ProgressStory(ProgressRequest progressRequest);
}