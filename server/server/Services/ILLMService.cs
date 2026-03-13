public interface ILLMService
{
    Task<string> Generate(string prompt);
    Task<bool> ValidateUserAction(string userAction, string storyStructure, string storySoFar);
}