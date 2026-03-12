public static class StoryEndpoints
{
    public static void MapStoryEndpoints(this WebApplication app)
    {
        app.MapGet("/random", async (ILLMService lLMService) =>
        {
            return await lLMService.Generate(
                "Tell a 10 word story. Only output the sentence."
            );
        });
    }
}