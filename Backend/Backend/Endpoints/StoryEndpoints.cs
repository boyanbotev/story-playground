using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Services;

public static class StoryEndpoints
{
    public static void MapStoryEndpoints(this WebApplication app)
    {
        app.MapPost("/stories", async ([FromBody] AddStoryRequest addStoryRequest,  IStoryService storyService, CancellationToken cancellationToken) =>
        {
            var result = await storyService.AddStory(addStoryRequest, cancellationToken);
            switch(result)
            {
                case AddResult.Success:
                    return Results.Created();
                case AddResult.AlreadyExists:
                    return Results.Conflict();
                case AddResult.Invalid:
                    return Results.BadRequest();
                default:
                    return Results.InternalServerError();
            }
        });

        app.MapDelete("/stories/{id}", async (int id, IStoryService storyService, CancellationToken cancellationToken) =>
        {
            var result = await storyService.DeleteStory(id, cancellationToken);
            switch(result)
            {
                case RemoveResult.Success:
                    return Results.Ok();
                case RemoveResult.DoesNotExist:
                    return Results.NotFound();
                default:
                    return Results.InternalServerError();
            }
        });

        app.MapGet("/stories/{id}", async (int id, IStoryService storyService, CancellationToken cancellationToken) =>
        {
            var story = await storyService.GetStory(id, cancellationToken);
            return Results.Ok(story);
        });

        app.MapGet("stories", async (IStoryService storyService, CancellationToken cancellationToken) =>
        {
            var stories = await storyService.GetStories(cancellationToken);
            return Results.Ok(stories);
        });

        app.MapPut("/stories/{id}", async (int id, [FromBody] UpdateStoryRequest updateStoryRequest, IStoryService storyService, CancellationToken cancellationToken) =>
        {
            var result = await storyService.UpdateStory(id, updateStoryRequest, cancellationToken);
            switch(result)
            {
                case UpdateResult.Success:
                    return Results.Ok();
                case UpdateResult.DoesNotExist:
                    return Results.NotFound();
                default:
                    return Results.InternalServerError();
            }
        });

        app.MapPost("/progress", async (ILLMService lLMService, IGameService gameService, [FromBody] ProgressRequest progressRequest, Settings settings, CancellationToken cancellationToken) =>
        {
            return await gameService.ProgressStory(progressRequest, cancellationToken);
        });
    }
}