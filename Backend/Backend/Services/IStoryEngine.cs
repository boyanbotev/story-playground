using Backend.Models.Db;
using Backend.Models.DTO;
using Backend.Services;

public interface IStoryEngine
{
    Task<StoryStatus> ProcessTurn(ProgressRequest progressRequest, Story story);
}