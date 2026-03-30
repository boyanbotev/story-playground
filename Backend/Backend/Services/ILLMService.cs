public interface ILLMService
{
    Task<string> Generate(string prompt, CancellationToken cancellationToken);
}