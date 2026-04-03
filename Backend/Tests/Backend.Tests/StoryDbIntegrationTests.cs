using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Net;
using Backend.Models.Db;
using Backend.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend.Tests;

public class StoryDbIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public StoryDbIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var _dbPath = Path.Join(Environment.GetFolderPath(folder), $"tests_{Guid.NewGuid()}.db");
        var dbConnectionString = $"Data Source={_dbPath}";

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<StoryContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<StoryContext>(optionsBuilder => 
                    optionsBuilder.UseSqlite(dbConnectionString).UseLoggerFactory(LoggerFactory.Create(builder => builder.ClearProviders()))
                );
                    
            });
        });

        _client = _factory.CreateClient();

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<StoryContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
        }
    }

    [Fact]
    public async Task ShouldGetEmptyListWhenNoStoriesExist()
    {
        var client = CreateAuthenticatedClient("testuser", "Testpassword1!");

        var response = await client.GetAsync("/stories");

        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<Story>>();
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task ShouldCreateStory()
    {
        var client = CreateAuthenticatedClient("testuser", "Testpassword1!");

        var addRequest = new AddStoryRequest
        {
            Name = "Test Story 1",
            StartingSummary = "Test Starting Summary",
            Structure = "Test Structure",
            Introduction = "Test Introduction",
            MainCharacterName = "Test Main Character Name",
            Nodes = new List<NodeRequest>{},
        };

        var content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/stories", content);

        response.EnsureSuccessStatusCode();

        var storiesResponse = await client.GetAsync("/stories");
        storiesResponse.EnsureSuccessStatusCode();
        var stories = await storiesResponse.Content.ReadFromJsonAsync<List<Story>>();
        Assert.NotNull(stories);
        Assert.Contains(stories, s => s.Name == addRequest.Name);
    }

    [Fact]
    public async Task ShouldDeleteStory()
    {
        var client = CreateAuthenticatedClient("testuser", "Testpassword1!");

        var addRequest = new AddStoryRequest
        {
            Name = $"Test Story {Guid.NewGuid()}",
            StartingSummary = "Test Starting Summary",
            Structure = "Test Structure",
            Introduction = "Test Introduction",
            MainCharacterName = "Test Main Character Name",
            Nodes = new List<NodeRequest>{},
        };

        var content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/stories", content);

        response.EnsureSuccessStatusCode();

        var storiesResponse = await client.GetAsync("/stories");
        storiesResponse.EnsureSuccessStatusCode();
        var stories = await storiesResponse.Content.ReadFromJsonAsync<List<Story>>();
        Assert.NotNull(stories);
        Assert.Single(stories);

        var existingStoryId = stories.Find(s => s.Name == addRequest.Name).Id;
        Console.WriteLine($"Existing Story Id: {existingStoryId}");

        var deleteResponse = await client.DeleteAsync($"/stories/{existingStoryId}");
        deleteResponse.EnsureSuccessStatusCode();

        var storiesResponseAfterDelete = await client.GetAsync("/stories");
        storiesResponseAfterDelete.EnsureSuccessStatusCode();
        var storiesAfterDelete = await storiesResponseAfterDelete.Content.ReadFromJsonAsync<List<Story>>();

        var storyAfterDelete = storiesAfterDelete.Find(s => s.Id == existingStoryId);
        Assert.Null(storyAfterDelete);
    }

    [Fact]
    public async Task ShouldUpdateStory()
    {
        var client = CreateAuthenticatedClient("testuser", "Testpassword1!");

        var addRequest = new AddStoryRequest
        {
            Name = $"Test Story {Guid.NewGuid()}",
            StartingSummary = "Test Starting Summary",
            Structure = "Test Structure",
            Introduction = "Test Introduction",
            MainCharacterName = "Test Main Character Name",
            Nodes = new List<NodeRequest>{},
        };

        var content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/stories", content);

        response.EnsureSuccessStatusCode();

        var storiesResponse = await client.GetAsync("/stories");
        var stories = await storiesResponse.Content.ReadFromJsonAsync<List<Story>>();
        var existingStoryId = stories.Find(s => s.Name == addRequest.Name).Id;

        var updateRequest = new UpdateStoryRequest
        {
            Name = $"{addRequest.Name} Updated",
            StartingSummary = "Test Starting Summary",
            Structure = "Test Structure",
            Introduction = "Test Introduction",
            MainCharacterName = "Test Main Character Name",
            Nodes = new List<NodeRequest>{},
        };
        var content2 = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");
        var updateResponse = await client.PutAsync($"/stories/{existingStoryId}", content2);
        updateResponse.EnsureSuccessStatusCode();
        var updatedStoriesResponse = await client.GetAsync("/stories");
        updatedStoriesResponse.EnsureSuccessStatusCode();
        var updatedStories = await updatedStoriesResponse.Content.ReadFromJsonAsync<List<Story>>();
        Assert.NotNull(updatedStories);
        Assert.Equal(updateRequest.Name, updatedStories.Find(s => s.Id == existingStoryId).Name);
    }

    [Fact]
    public async Task ShouldGetStory()
    {
        var client = CreateAuthenticatedClient("testuser", "Testpassword1!");
        var addRequest = new AddStoryRequest
        {
            Name = $"Test Story {Guid.NewGuid()}",
            StartingSummary = "Test Starting Summary",
            Structure = "Test Structure",
            Introduction = "Test Introduction",
            MainCharacterName = "Test Main Character Name",
            Nodes = new List<NodeRequest>{},
        };
        var content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/stories", content);
        response.EnsureSuccessStatusCode();
        var storiesResponse = await client.GetAsync("/stories");
        var stories = await storiesResponse.Content.ReadFromJsonAsync<List<Story>>();
        Assert.NotNull(stories);
        Assert.Single(stories);
        var existingStoryId = stories.Find(s => s.Name == addRequest.Name).Id;
        var getResponse = await client.GetAsync($"/stories/{existingStoryId}");
        getResponse.EnsureSuccessStatusCode();
        var getStory = await getResponse.Content.ReadFromJsonAsync<Story>();
        Assert.NotNull(getStory);
        Assert.Equal(addRequest.Name, getStory.Name);
    }

    [Fact]
    public async Task ShouldGetStoryWithNodes()
    {
        var client = CreateAuthenticatedClient("testuser", "Testpassword1!");
        var addRequest = new AddStoryRequest
        {
            Name = $"Test Story {Guid.NewGuid()}",
            StartingSummary = "Test Starting Summary",
            Structure = "Test Structure",
            Introduction = "Test Introduction",
            MainCharacterName = "Test Main Character Name",
            Nodes = new List<NodeRequest>
            {
                new StoryNodeRequest
                {
                    Content = "Test Node 1 Content",
                    ContentTurns = 1,
                    TransitionTurns = 1,
                },
                new QuestNodeRequest
                {
                    UserGoal = "Test User Goal",
                    Difficulty = "Test Difficulty",
                },
            },
        };
        var content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/stories", content);
        response.EnsureSuccessStatusCode();
        var storiesResponse = await client.GetAsync("/stories");
        var stories = await storiesResponse.Content.ReadFromJsonAsync<List<Story>>();
        Assert.NotNull(stories);
        Assert.Single(stories);
        var existingStoryId = stories.Find(s => s.Name == addRequest.Name).Id;
        var getResponse = await client.GetAsync($"/stories/{existingStoryId}");
        getResponse.EnsureSuccessStatusCode();
        var getStory = await getResponse.Content.ReadFromJsonAsync<Story>();
        Assert.NotNull(getStory);
        Assert.Equal(addRequest.Name, getStory.Name);
        Assert.Equal((addRequest.Nodes.First() as StoryNodeRequest).Content, (getStory.Nodes.OrderBy(n => n.Order).First() as StoryNode).Content);
        Assert.Equal((addRequest.Nodes.Last() as QuestNodeRequest).UserGoal, (getStory.Nodes.OrderBy(n => n.Order).Last() as QuestNode).UserGoal);
    }

    [Fact]
    public async Task ShouldNotUpdateOtherUserTasks()
    {
        var client = CreateAuthenticatedClient("testuser", "Testpassword1!");
        var client2 = CreateAuthenticatedClient("testuser2", "Testpassword2!");

        var addRequest = new AddStoryRequest
        {
            Name = $"Test Story {Guid.NewGuid()}",
            StartingSummary = "Test Starting Summary",
            Structure = "Test Structure",
            Introduction = "Test Introduction",
            MainCharacterName = "Test Main Character Name",
            Nodes = new List<NodeRequest>{},
        };

        var content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/stories", content);

        response.EnsureSuccessStatusCode();

        var storiesResponse = await client.GetAsync("/stories");
        storiesResponse.EnsureSuccessStatusCode();
        var stories = await storiesResponse.Content.ReadFromJsonAsync<List<Story>>();
        Assert.NotNull(stories);
        Assert.Single(stories);

        var existingStoryId = stories.Find(s => s.Name == addRequest.Name).Id;

        var updateRequest = new UpdateStoryRequest
        {
            Name = $"{addRequest.Name} Updated",
            StartingSummary = "Test Starting Summary",
            Structure = "Test Structure",
            Introduction = "Test Introduction",
            MainCharacterName = "Test Main Character Name",
            Nodes = new List<NodeRequest>{},
        };
        var content2 = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");
        var updateResponse = await client2.PutAsync($"/stories/{existingStoryId}", content2);
        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }

    [Fact]
    public async Task ShouldRejectAnonymousRequests()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/stories");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<string> GetAuthToken(string username, string password)
    {
        var registerRequest = new { username, password };
        var registerContent = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json");
        await _client.PostAsync("/auth/register", registerContent);

        var loginRequest = new { username, password };
        var loginContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");
        var loginResponse = await _client.PostAsync("/auth/login", loginContent);
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return loginResult["token"];
    }

    private HttpClient CreateAuthenticatedClient(string username, string password)
    {
        var token = GetAuthToken(username, password).Result;
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public void Dispose()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<StoryContext>();
            db.Database.CloseConnection();
        }
    }
}
