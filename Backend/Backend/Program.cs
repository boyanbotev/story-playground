using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Backend.Services;
using Backend.Models;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IPromptService, PromptService>();
builder.Services.AddScoped<ILLMService, OllamaService>();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
        options.SerializerOptions.TypeInfoResolverChain.Insert(
        0,
        new DefaultJsonTypeInfoResolver()
    );
});

var settings = new Settings();
builder.Configuration.Bind("Settings", settings);
builder.Services.AddSingleton(settings);

var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var dbPath = Path.Join(path, "stories.db");

builder.Services.AddDbContext<Backend.Models.Db.StoryContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.MapStoryEndpoints();

app.Run();
