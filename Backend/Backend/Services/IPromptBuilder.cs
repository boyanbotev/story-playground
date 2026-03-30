using Backend.Models.DTO;
using Backend.Models.Db;

public interface IPromptBuilder
{
    string CreateQuestPrompt(ProgressRequest progressRequest, QuestNode questNode);
    string CreateStoryPrompt(ProgressRequest progressRequest, StoryNode storyNode);
}