
namespace Backend.Services;
public class PromptService: IPromptService
{
    public string Load(string name)
    {
        return File.ReadAllText($"Prompts/{name}.txt");
    }

    public string Fill(string template, Dictionary<string, string> values)
    {
        foreach (var kv in values)
        {
            template = template.Replace($"{{{{{kv.Key}}}}}", kv.Value);
        }
        return template;
    }
}