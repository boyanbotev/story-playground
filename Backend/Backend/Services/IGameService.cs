using Backend.Models.DTO;

public interface IGameService
{
    Task<ProgressResponse> ProgressStory(ProgressRequest progressRequest, string userId, string apiKey, CancellationToken cancellationToken);
}