using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Services;
using Backend.Models.Db;
using System.Security.Claims;

public static class StoryEndpoints
{
    public static void MapStoryEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/register", async ([FromBody] RegisterRequest registerRequest, IAuthService authService, CancellationToken cancellationToken) =>
        {
            var (succeeded, token, errors) = await authService.Register(registerRequest);
            if (succeeded)
            {
                return Results.Ok(new { token });
            }
            return Results.BadRequest(errors);
        })
        .WithName("RegisterUser")
        .Produces<string>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPost("/auth/login", async ([FromBody] LoginRequest loginRequest, IAuthService authService, CancellationToken cancellationToken) =>
        {
            var (succeeded, token) = await authService.Login(loginRequest);

            if (succeeded)
            {
                return Results.Ok(new { token });
            }

            return Results.Unauthorized();
        })
        .WithName("LoginUser")
        .Produces<string>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        app.MapPost("/stories", [Authorize] async ([FromBody] AddStoryRequest addStoryRequest,  IStoryService storyService,  ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var userId = user.FindFirstValue("UserId");
            var result = await storyService.AddStory(addStoryRequest, userId, cancellationToken);
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
        })
        .WithName("AddStory")
        .Produces<Story>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status500InternalServerError);

        app.MapDelete("/stories/{id}", [Authorize] async (int id, IStoryService storyService, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var userId = user.FindFirstValue("UserId");
            var result = await storyService.DeleteStory(id, userId, cancellationToken);
            switch(result)
            {
                case RemoveResult.Success:
                    return Results.Ok();
                case RemoveResult.DoesNotExist:
                    return Results.NotFound();
                default:
                    return Results.InternalServerError();
            }
        })
        .WithName("DeleteStory")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        app.MapGet("/stories/{id}", [Authorize] async (int id, IStoryService storyService, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var userId = user.FindFirstValue("UserId");
            var story = await storyService.GetStory(id, userId, cancellationToken);
            return Results.Ok(story);
        })
        .WithName("GetStoryById")
        .Produces<Story>(StatusCodes.Status200OK);

        app.MapGet("stories", [Authorize] async (IStoryService storyService, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var userId = user.FindFirstValue("UserId");
            var stories = await storyService.GetStories(userId, cancellationToken);
            return Results.Ok(stories);
        })
        .WithName("GetAllStories")
        .Produces<Story[]>(StatusCodes.Status200OK); 

        app.MapPut("/stories/{id}", [Authorize] async (int id, [FromBody] UpdateStoryRequest updateStoryRequest, IStoryService storyService, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var userId = user.FindFirstValue("UserId");
            var result = await storyService.UpdateStory(id, userId, updateStoryRequest, cancellationToken);
            switch(result)
            {
                case UpdateResult.Success:
                    return Results.Ok();
                case UpdateResult.DoesNotExist:
                    return Results.NotFound();
                default:
                    return Results.InternalServerError();
            }
        })
        .WithName("UpdateStory")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        app.MapPost("/progress", [Authorize] async (ILLMService lLMService, IGameService gameService, [FromBody] ProgressRequest progressRequest, Settings settings, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var userId = user.FindFirstValue("UserId");
            return await gameService.ProgressStory(progressRequest, userId, cancellationToken);
        });
    }
}