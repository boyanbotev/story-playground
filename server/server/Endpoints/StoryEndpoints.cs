using Microsoft.AspNetCore.Mvc;
using server.models;
using server.models.DTO;

public static class StoryEndpoints
{
    public static void MapStoryEndpoints(this WebApplication app, Settings settings)
    {

        app.MapPost("/progress", async (ILLMService lLMService, [FromBody] ProgressRequest progressRequest) =>
        {
            bool isValid = await lLMService.ValidateUserAction(progressRequest.UserAction, progressRequest.StoryStructure, progressRequest.SummarySoFar);
            if (!isValid)
            {
                return "Invalid User Action";
            }

            string steeringInstruction = progressRequest.TurnsRemaining switch
            {
                0 => $"CRITICAL INSTRUCTION: The user MUST now experience this exact event: '{progressRequest.NextNode}'. Make it happen in this response.",
                
                <= 2 => $"CRITICAL INSTRUCTION: Transition the scene to set up this upcoming event: '{progressRequest.NextNode}'. DO NOT let the event actually happen yet. Just put the pieces in place based on the User Action.",
                
                _ => $"CRITICAL INSTRUCTION: Focus ENTIRELY on the User Action. Do NOT introduce any new characters or major events yet."
            };

            string upcomingEventContext = progressRequest.TurnsRemaining <= 2 
                ? $"\nUpcoming Event to foreshadow: {progressRequest.NextNode}" 
                : "";

            string prompt = $@"{settings.SystemPrompt}
Story Style Guide: {settings.StyleGuide}

Summary of story so far: {progressRequest.SummarySoFar}
{upcomingEventContext}

=== CURRENT TURN ===
User Action: {progressRequest.UserAction}
{steeringInstruction}

Write the next 40-70 words now:";

            Console.WriteLine("Prompt:\n" + prompt);
            return await lLMService.Generate(prompt);
        });
    }
}