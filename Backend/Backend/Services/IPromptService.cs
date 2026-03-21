public interface IPromptService
{
    string Load(string name);
    string Fill(string template, Dictionary<string, string> values);
}