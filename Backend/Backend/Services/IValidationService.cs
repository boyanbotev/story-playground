using Backend.Models.DTO;
using Backend.Models.Db;

public interface IValidationService
{
    Task<bool> ValidateUserAction(ProgressRequest progressRequest, Story story, CancellationToken cancellationToken);
    Task<bool> Validate(string prompt, CancellationToken cancellationToken);
    Task<bool> ValidateGoalReached(string textToCheck, string characterGoal, string storySoFar, CancellationToken cancellationToken);
}