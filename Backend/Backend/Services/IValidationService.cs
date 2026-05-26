using Backend.Models.DTO;
using Backend.Models.Db;

namespace Backend.Services;

public interface IValidationService
{
    Task<ValidationResponse> ValidateUserAction(ProgressRequest progressRequest, Story story, string apiKey, CancellationToken cancellationToken);
    Task<ValidationResponse> Validate(string prompt, string apiKey, CancellationToken cancellationToken);
    Task<ValidationResponse> ValidateGoalReached(string textToCheck, string characterGoal, string storySoFar, string apiKey, CancellationToken cancellationToken);
}