using Microsoft.AspNetCore.Mvc;
using server.models;
using server.models.DTO;
using server.Services;
using System.Net;

public static class StoryEndpoints
{
    public static void MapStoryEndpoints(this WebApplication app)
    {
        app.MapPost("/stories", async ([FromBody] AddStoryRequest addStoryRequest,  IStoryService storyService) =>
        {
            var result = await storyService.AddStory(addStoryRequest);
            switch(result)
            {
                case AddResult.Success:
                    return HttpStatusCode.Created;
                case AddResult.AlreadyExists:
                    return HttpStatusCode.Conflict;
                case AddResult.Invalid:
                    return HttpStatusCode.BadRequest;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        });

        app.MapDelete("/stories/{id}", async (int id, IStoryService storyService) =>
        {
            var result = await storyService.DeleteStory(id);
            switch(result)
            {
                case RemoveResult.Success:
                    return HttpStatusCode.OK;
                case RemoveResult.DoesNotExist:
                    return HttpStatusCode.NotFound;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        });

        app.MapGet("/stories/{id}", async (int id, IStoryService storyService) =>
        {
            var story = await storyService.GetStory(id);
            return Results.Ok(story);
        });

        app.MapGet("stories", async (IStoryService storyService) =>
        {
            var stories = await storyService.GetStories();
            return Results.Ok(stories);
        });

        app.MapPut("/stories/{id}", async (int id, [FromBody] UpdateStoryRequest updateStoryRequest, IStoryService storyService) =>
        {
            var result = await storyService.UpdateStory(id, updateStoryRequest);
            switch(result)
            {
                case UpdateResult.Success:
                    return HttpStatusCode.OK;
                case UpdateResult.DoesNotExist:
                    return HttpStatusCode.NotFound;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        });

        app.MapPost("/progress", async (ILLMService lLMService, IStoryService storyService, [FromBody] ProgressRequest progressRequest, Settings settings) =>
        {
            return await storyService.ProgressStory(progressRequest);
        });
    }
}