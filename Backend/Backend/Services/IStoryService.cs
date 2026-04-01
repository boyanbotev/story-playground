using Backend.Models.DTO;
using Backend.Services;
using Backend.Models.Db;

public interface IStoryService
{
    Task<AddResult> AddStory(AddStoryRequest addStoryRequest, string userId, CancellationToken cancellationToken);
    Task<Story> GetStory(int id, string userId, CancellationToken cancellationToken);
    Task<UpdateResult> UpdateStory(int id,  string userId, UpdateStoryRequest updateStoryRequest, CancellationToken cancellationToken);
    Task<RemoveResult> DeleteStory(int id, string userId, CancellationToken cancellationToken);
    Task<Story[]> GetStories(string userId, CancellationToken cancellationToken);
}