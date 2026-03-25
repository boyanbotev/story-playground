using Backend.Models.DTO;

public interface IGameService
{
    Task<object> ProgressStory(ProgressRequest progressRequest);
}