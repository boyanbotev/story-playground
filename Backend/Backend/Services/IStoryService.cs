using Backend.Models.DTO;
using Backend.Services;
using Backend.Models.Db;

public interface IStoryService
{
    Task<AddResult> AddStory(AddStoryRequest addStoryRequest, CancellationToken cancellationToken);
    Task<Story> GetStory(int id, CancellationToken cancellationToken);
    Task<UpdateResult> UpdateStory(int id, UpdateStoryRequest updateStoryRequest, CancellationToken cancellationToken);
    Task<RemoveResult> DeleteStory(int id, CancellationToken cancellationToken);
    Task<Story[]> GetStories(CancellationToken cancellationToken);
}