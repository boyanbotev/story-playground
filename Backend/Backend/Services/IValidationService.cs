using Backend.Models.DTO;
using Backend.Models.Db;

namespace Backend.Services;

public interface IValidationService
{
    Task<ValidationResponse> ValidateUserAction(ProgressRequest progressRequest, Story story, CancellationToken cancellationToken);
    Task<ValidationResponse> Validate(string prompt, CancellationToken cancellationToken);
    Task<ValidationResponse> ValidateGoalReached(string textToCheck, string characterGoal, string storySoFar, CancellationToken cancellationToken);
}