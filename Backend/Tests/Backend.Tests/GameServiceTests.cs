using Moq;
using Backend.Models.Db;
using Backend.Models.DTO;
using Backend.Services;

namespace Backend.Tests;

public class GameServiceTests
{
    private readonly Mock<ILLMService> _mockLlmService;
    private readonly Mock<IStoryService> _mockStoryService;
    private readonly Mock<IValidationService> _mockValidationService;
    private readonly Mock<ISummaryService> _mockSummaryService;
    private readonly Mock<IStoryEngine> _mockStoryEngine;
    private readonly GameService _sut;

    private static readonly CancellationToken None = CancellationToken.None;

    public GameServiceTests()
    {
        _mockLlmService  = new Mock<ILLMService>();
        _mockStoryService = new Mock<IStoryService>();
        _mockValidationService = new Mock<IValidationService>();
        _mockSummaryService = new Mock<ISummaryService>();
        _mockStoryEngine = new Mock<IStoryEngine>();

        _sut = new GameService(
            _mockLlmService.Object,
            _mockStoryService.Object,
            _mockValidationService.Object,
            _mockSummaryService.Object,
            _mockStoryEngine.Object
        );
    }

    // private static Story MakeStory() => new Story { Id = 1 };

    // private static ProgressRequest MakeRequest(string summarySoFar = "So far...") => new ProgressRequest
    // {
    //     StoryId = 1,
    //     NodeIndex = 0,
    //     UserAction = "I open the door",
    //     SummarySoFar = summarySoFar
    // };

    // private void SetupHappyPath(Story story, ProgressRequest request, ProgressResponse engineResponse)
    // {
    //     _mockStoryService
    //         .Setup(s => s.GetStory(request.StoryId, None))
    //         .ReturnsAsync(story);

    //     _mockValidationService
    //         .Setup(v => v.ValidateUserAction(request, story, None))
    //         .ReturnsAsync(true);

    //     _mockStoryEngine
    //         .Setup(e => e.ProcessTurn(request, story, None))
    //         .ReturnsAsync(engineResponse);
    // }

    // [Fact]
    // public async Task ProgressStoryWhenActionInvalidReturnsErrorResponse()
    // {
    //     var story = MakeStory();
    //     var request = MakeRequest();

    //     _mockStoryService
    //         .Setup(s => s.GetStory(request.StoryId, None))
    //         .ReturnsAsync(story);

    //     _mockValidationService
    //         .Setup(v => v.ValidateUserAction(request, story, None))
    //         .ReturnsAsync(false);

    //     var result = await _sut.ProgressStory(request, None);

    //     Assert.Equal("Invalid User Action", result.Error);
    // }

    // [Fact]
    // public async Task ProgressStoryWhenActionInvalidDoesNotCallStoryEngine()
    // {
    //     var story = MakeStory();
    //     var request = MakeRequest();

    //     _mockStoryService
    //         .Setup(s => s.GetStory(request.StoryId, None))
    //         .ReturnsAsync(story);

    //     _mockValidationService
    //         .Setup(v => v.ValidateUserAction(request, story, None))
    //         .ReturnsAsync(false);

    //     await _sut.ProgressStory(request, None);

    //     _mockStoryEngine.Verify(e => e.ProcessTurn(It.IsAny<ProgressRequest>(), It.IsAny<Story>(), It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ProgressStoryWhenNotCompletedGeneratesAndAttachesSummary()
    // {
    //     var story = MakeStory();
    //     var request = MakeRequest("Intro.");
    //     var engineResponse = new ProgressResponse
    //     {
    //         StoryText = "You open the door and step inside.",
    //         Completed = false
    //     };
    //     const string newSummary = "Intro. You opened the door.";

    //     SetupHappyPath(story, request, engineResponse);

    //     _mockSummaryService
    //         .Setup(s => s.GenerateSummary(request, engineResponse.StoryText, None))
    //         .ReturnsAsync(newSummary);

    //     var result = await _sut.ProgressStory(request, None);

    //     Assert.Equal(newSummary, result.SummarySoFar);
    // }

    // [Fact]
    // public async Task ProgressStoryWhenCompletedKeepsExistingSummary()
    // {
    //     var story = MakeStory();
    //     var request = MakeRequest("The whole story.");
    //     var engineResponse = new ProgressResponse
    //     {
    //         StoryText = "The end.",
    //         Completed = true
    //     };

    //     SetupHappyPath(story, request, engineResponse);

    //     var result = await _sut.ProgressStory(request, None);

    //     Assert.Equal(request.SummarySoFar, result.SummarySoFar);
    //     _mockSummaryService.Verify(s => s.GenerateSummary(It.IsAny<ProgressRequest>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ProgressStoryReturnsEngineResponseFields()
    // {
    //     var story = MakeStory();
    //     var request = MakeRequest();
    //     var engineResponse = new ProgressResponse
    //     {
    //         StoryText = "Some narrative.",
    //         NodeIndex = 1,
    //         Completed = false
    //     };

    //     SetupHappyPath(story, request, engineResponse);
    //     _mockSummaryService
    //         .Setup(s => s.GenerateSummary(request, engineResponse.StoryText, None))
    //         .ReturnsAsync("updated summary");

    //     var result = await _sut.ProgressStory(request, None);

    //     Assert.Equal("Some narrative.", result.StoryText);
    //     Assert.Equal(1, result.NodeIndex);
    //     Assert.False(result.Completed);
    // }

    // [Fact]
    // public async Task ProgressStoryFetchesCorrectStory()
    // {
    //     var story = MakeStory();
    //     var request = MakeRequest();
    //     var engineResponse = new ProgressResponse { StoryText = "x", Completed = false };

    //     SetupHappyPath(story, request, engineResponse);
    //     _mockSummaryService
    //         .Setup(s => s.GenerateSummary(It.IsAny<ProgressRequest>(), It.IsAny<string>(), None))
    //         .ReturnsAsync("s");

    //     await _sut.ProgressStory(request, None);

    //     _mockStoryService.Verify(s => s.GetStory(1, None), Times.Once);
    // }
}