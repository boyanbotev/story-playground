using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Backend.Services;
using Backend.Models;
using Backend.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Logging.AddConsole();
builder.Services.AddScoped<IPromptService, PromptService>();
builder.Services.AddScoped<ILLMService, OllamaService>();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddScoped<IPromptBuilder, PromptBuilder>();
builder.Services.AddScoped<IStoryEngine, StoryEngine>();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
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
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

//app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapStoryEndpoints();

app.Run();
