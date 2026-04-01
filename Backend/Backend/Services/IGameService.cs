using Backend.Models.DTO;

public interface IGameService
{
    Task<ProgressResponse> ProgressStory(ProgressRequest progressRequest, string userId, CancellationToken cancellationToken);
}