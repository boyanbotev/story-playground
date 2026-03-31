using Backend.Models.DTO;

public interface IGameService
{
    Task<ProgressResponse> ProgressStory(ProgressRequest progressRequest, CancellationToken cancellationToken);
}