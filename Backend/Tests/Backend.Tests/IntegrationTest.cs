using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Net;
using Backend.Models.Db;
using Backend.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests;

public class TasksControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TasksControllerIntegrationTests(WebApplicationFactory<Program> factory)
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

                services.AddDbContext<StoryContext>(options =>
                    options.UseSqlite(dbConnectionString));
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
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/stories");

        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<Story>>();
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task ShouldCreateStory()
    {
        var client = _factory.CreateClient();

        var addRequest = new AddStoryRequest
        {
            Name = "Test Story",
            StartingSummary = "Test Starting Summary",
            Structure = "Test Structure",
            Introduction = "Test Introduction",
            MainCharacterName = "Test Main Character Name",
            Nodes = new List<NodeRequest>{},
        };

        var content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/stories", content);

        response.EnsureSuccessStatusCode();

        var tasksResponse = await client.GetAsync("/stories");
        tasksResponse.EnsureSuccessStatusCode();
        var tasks = await tasksResponse.Content.ReadFromJsonAsync<List<Story>>();
        Assert.NotNull(tasks);
        Assert.Single(tasks);
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
