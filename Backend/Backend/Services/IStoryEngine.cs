using Backend.Models.Db;
using Backend.Models.DTO;

public interface IStoryEngine
{
    Task<ProgressResponse> ProcessTurn(ProgressRequest progressRequest, Story story, CancellationToken cancellationToken);
}