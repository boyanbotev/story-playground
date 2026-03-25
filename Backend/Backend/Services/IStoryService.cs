using Backend.Models.DTO;
using Backend.Services;
using Backend.Models.Db;

public interface IStoryService
{
    Task<AddResult> AddStory(AddStoryRequest addStoryRequest);
    Task<Story> GetStory(int id);
    Task<UpdateResult> UpdateStory(int id, UpdateStoryRequest updateStoryRequest);
    Task<RemoveResult> DeleteStory(int id);
    Task<Story[]> GetStories();
}