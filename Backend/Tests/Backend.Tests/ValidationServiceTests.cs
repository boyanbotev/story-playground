using Backend.Models.Db;
using Backend.Models.DTO;
using Moq;
using Backend.Services;
using Microsoft.Extensions.Logging;

public class ValidationServiceTests
{
    private readonly Mock<ILLMService> _mockLlmService;
    private readonly Mock<IPromptService> _mockPromptService;
    private readonly Mock<ILogger<ValidationService>> _mockLogger;
    private readonly ValidationService _sut;

    private static readonly CancellationToken None = CancellationToken.None;

    public ValidationServiceTests()
    {
        _mockLlmService = new Mock<ILLMService>();
        _mockPromptService = new Mock<IPromptService>();
        _mockLogger = new Mock<ILogger<ValidationService>>();

        _sut = new ValidationService(_mockLlmService.Object, _mockPromptService.Object, _mockLogger.Object);
    }

    private void SetupPrompt(string templateName, string returnedPrompt = "prompt")
    {
        _mockPromptService.Setup(p => p.Load(templateName)).Returns(templateName);
        _mockPromptService.Setup(p => p.Fill(templateName, It.IsAny<Dictionary<string, string>>())).Returns(returnedPrompt);
    }

    private void SetupLlm(string response)
    {
        _mockLlmService.Setup(l => l.Generate(It.IsAny<string>(), None)).ReturnsAsync(response);
    }

    [Theory]
    [InlineData("YES")]
    [InlineData("yes")]
    [InlineData("Yes")]
    [InlineData("  YES  ")]
    [InlineData("  yes\n")]
    public async Task ValidateReturnsTrueForYesVariants(string llmResponse)
    {
        SetupLlm(llmResponse);
        Assert.True(await _sut.Validate("any prompt", None));
    }

    [Theory]
    [InlineData("NO")]
    [InlineData("no")]
    [InlineData("No")]
    [InlineData("  NO  ")]
    [InlineData("  no\n")]
    public async Task ValidateReturnsFalseForNoVariants(string llmResponse)
    {
        SetupLlm(llmResponse);
        Assert.False(await _sut.Validate("any prompt", None));
    }

    [Theory]
    [InlineData("YES.")]
    [InlineData("Yes, definitely")]
    [InlineData("NO!")]
    [InlineData("Maybe")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ValidateThrowsWhenResponseIsNotYesOrNo(string llmResponse)
    {
        SetupLlm(llmResponse);
        await Assert.ThrowsAsync<Exception>(() => _sut.Validate("any prompt", None));
    }

    [Fact]
    public async Task ValidateExceptionMessageContainsPromptAndResult()
    {
        SetupLlm("definitely yes");
        var ex = await Record.ExceptionAsync(() => _sut.Validate("my prompt", None));
        Assert.Contains("my prompt", ex.Message);
        Assert.Contains("definitely yes", ex.Message);
    }

    [Fact]
    public async Task ValidateGoalReachedReturnsTrueWhenLlmSaysYes()
    {
        SetupPrompt("validate_goal_reached");
        SetupLlm("YES");
        Assert.True(await _sut.ValidateGoalReached("text", "goal", "summary", None));
    }

    [Fact]
    public async Task ValidateGoalReachedReturnsFalseWhenLlmSaysNo()
    {
        SetupPrompt("validate_goal_reached");
        SetupLlm("NO");
        Assert.False(await _sut.ValidateGoalReached("text", "goal", "summary", None));
    }

    [Fact]
    public async Task ValidateGoalReachedFillsTemplateWithCorrectKeys()
    {
        SetupPrompt("validate_goal_reached");
        SetupLlm("YES");

        await _sut.ValidateGoalReached("the text", "the goal", "the summary", None);

        _mockPromptService.Verify(p => p.Fill(
            It.IsAny<string>(),
            It.Is<Dictionary<string, string>>(d =>
                d["TextToCheck"] == "the text" &&
                d["CharacterGoal"] == "the goal" &&
                d["StorySoFar"] == "the summary"
            )), Times.Once);
    }

    [Fact]
    public async Task ValidateUserActionReturnsFalseWhenPlausibilityFails()
    {
        var story = new Story { Structure = "structure", MainCharacterName = "Hero" };
        var request = new ProgressRequest { UserAction = "fly to the moon", SummarySoFar = "summary" };

        // plausibility → NO, character → YES
        SetupPrompt("validate_action_plausibility");
        SetupPrompt("validate_action_character");
        _mockLlmService
            .SetupSequence(l => l.Generate(It.IsAny<string>(), None))
            .ReturnsAsync("NO")
            .ReturnsAsync("YES");

        Assert.False(await _sut.ValidateUserAction(request, story, None));
    }

    [Fact]
    public async Task ValidateUserActionReturnsFalseWhenCharacterCheckFails()
    {
        var story = new Story { Structure = "structure", MainCharacterName = "Hero" };
        var request = new ProgressRequest { UserAction = "villain attacks", SummarySoFar = "summary" };

        SetupPrompt("validate_action_plausibility");
        SetupPrompt("validate_action_character");
        _mockLlmService
            .SetupSequence(l => l.Generate(It.IsAny<string>(), None))
            .ReturnsAsync("YES")
            .ReturnsAsync("NO");

        Assert.False(await _sut.ValidateUserAction(request, story, None));
    }

    [Fact]
    public async Task ValidateUserActionReturnsTrueWhenBothChecksPass()
    {
        var story = new Story { Structure = "structure", MainCharacterName = "Hero" };
        var request = new ProgressRequest { UserAction = "Hero opens door", SummarySoFar = "summary" };

        SetupPrompt("validate_action_plausibility");
        SetupPrompt("validate_action_character");
        _mockLlmService
            .SetupSequence(l => l.Generate(It.IsAny<string>(), None))
            .ReturnsAsync("YES")
            .ReturnsAsync("YES");

        Assert.True(await _sut.ValidateUserAction(request, story, None));
    }
}