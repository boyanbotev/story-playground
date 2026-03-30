using Backend.Models.DTO;
using Backend.Models.Db;

public interface IValidationService
{
    Task<bool> ValidateUserAction(ProgressRequest progressRequest, Story story);
}