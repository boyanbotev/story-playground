using Backend.Models.Db;
using Backend.Models.DTO;
using Moq;
using Backend.Services;
using Microsoft.Extensions.Logging;

public class StoryEngineTests
{
    private readonly Mock<IPromptBuilder> _mockPromptBuilder;
    private readonly Mock<ILLMService> _mockLlmService;
    private readonly Mock<IValidationService> _mockValidationService;
    private readonly StoryEngine _sut;

    private static readonly CancellationToken None = CancellationToken.None;

    public StoryEngineTests()
    {
        _mockPromptBuilder = new Mock<IPromptBuilder>();
        _mockLlmService = new Mock<ILLMService>();
        _mockValidationService = new Mock<IValidationService>();

        _sut = new StoryEngine(_mockPromptBuilder.Object, _mockLlmService.Object, _mockValidationService.Object);
    }

    private void SetupPromptBuilder(string templateName)
    {
        _mockPromptBuilder.Setup(p => p.CreateStoryPrompt(It.IsAny<ProgressRequest>(), It.IsAny<StoryNode>())).Returns(templateName);
        _mockPromptBuilder.Setup(p => p.CreateQuestPrompt(It.IsAny<ProgressRequest>(), It.IsAny<QuestNode>())).Returns(templateName);
    }

    private void SetupValidationService(bool isGoalReached)
    {
        _mockValidationService.Setup(v => v.ValidateGoalReached(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), None)).ReturnsAsync(isGoalReached);
    }

    private void SetupLlm(string response)
    {
        _mockLlmService.Setup(l => l.Generate(It.IsAny<string>(), None)).ReturnsAsync(response);
    }

    [Fact]
    public void GetStatusIncrementsTransitionTurnsBeforeContentTurns()
    {
        var story = new Story { Nodes = new List<Node> { new StoryNode { Content = "content test", TransitionTurns = 1, ContentTurns = 1 } } };
        var progressRequest = new ProgressRequest { NodeIndex = 0, TransitionTurnsRemaining = 1, ContentTurnsRemaining = 1 };

        SetupPromptBuilder("progress");
        SetupLlm("content test");
        SetupValidationService(false);

        var status = _sut.ProcessTurn(progressRequest, story, None).Result;

        Assert.Equal(0, status.TransitionTurnsRemaining);
        Assert.Equal(1, status.ContentTurnsRemaining);
    }

    [Fact]
    public void GetStatusIncrementsContentTurns()
    {
        var story = new Story { Nodes = new List<Node> { new StoryNode { Content = "content test", TransitionTurns = 1, ContentTurns = 1 } } };
        var progressRequest = new ProgressRequest { NodeIndex = 0, TransitionTurnsRemaining = 0, ContentTurnsRemaining = 2 };

        SetupPromptBuilder("progress");
        SetupLlm("content test");
        SetupValidationService(false);

        var status = _sut.ProcessTurn(progressRequest, story, None).Result;

        Assert.Equal(0, status.TransitionTurnsRemaining);
        Assert.Equal(1, status.ContentTurnsRemaining);
    }

    [Fact]
    public void GetStatusGoesToNextNodeWhenContentTurnsHitZero()
    {
        var story = new Story { Nodes = new List<Node> { new StoryNode { Content = "content test", TransitionTurns = 1, ContentTurns = 1 }, new StoryNode { Content = "next content test", TransitionTurns = 1, ContentTurns = 1 } } };
        var progressRequest = new ProgressRequest { NodeIndex = 0, TransitionTurnsRemaining = 0, ContentTurnsRemaining = 1 };

        SetupPromptBuilder("progress");
        SetupLlm("content test");
        SetupValidationService(false);

        var status = _sut.ProcessTurn(progressRequest, story, None).Result;

        Assert.Equal(1, status.TransitionTurnsRemaining);
        Assert.Equal(1, status.ContentTurnsRemaining);
        Assert.Equal(1, status.NodeIndex);
    }

    [Fact]
    public void GetStatusReturnsCompletedWhenCompletedLastNode()
    {
        var story = new Story { Nodes = new List<Node> { new StoryNode { Content = "content test", TransitionTurns = 1, ContentTurns = 1 } } };
        var progressRequest = new ProgressRequest { NodeIndex = 0, TransitionTurnsRemaining = 0, ContentTurnsRemaining = 1 };

        SetupPromptBuilder("progress");
        SetupLlm("content test");
        SetupValidationService(false);

        var status = _sut.ProcessTurn(progressRequest, story, None).Result;

        Assert.True(status.Completed);
    }

    [Fact]
    public void GetStatusReturnsQuestCompleteTextWhenGoalReached()
    {
        var story = new Story { Nodes = new List<Node> { new QuestNode { UserGoal = "user goal", Difficulty = "difficulty" } } };
        var progressRequest = new ProgressRequest { NodeIndex = 0 };

        SetupPromptBuilder("progress");
        SetupLlm("content test");
        SetupValidationService(true);

        var status = _sut.ProcessTurn(progressRequest, story, None).Result;

        Assert.Equal("QUEST COMPLETE: user goal", status.QuestCompleteText);
    }

    [Fact]
    public void GetStatusReturnsSameResponseWhenGoalNotReached()
    {
        var story = new Story { Nodes = new List<Node> { new QuestNode { UserGoal = "user goal", Difficulty = "difficulty" } } };
        var progressRequest = new ProgressRequest { NodeIndex = 0 };

        SetupPromptBuilder("progress");
        SetupLlm("content test");
        SetupValidationService(false);

        var status = _sut.ProcessTurn(progressRequest, story, None).Result;

        Assert.Equal(0, status.NodeIndex);
        Assert.Equal("user goal", status.UserGoal);
        Assert.Equal("difficulty", status.Difficulty);
    }
}