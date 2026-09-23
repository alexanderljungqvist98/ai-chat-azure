using CharactherGenerator.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var endpoint = builder.Configuration["AzureAI:Endpoint"]
    ?? throw new InvalidOperationException("AzureAI:Endpoint saknas.");

var apiKey = builder.Configuration["AzureAI:ApiKey"]
    ?? throw new InvalidOperationException("AzureAI:ApiKey saknas.");

builder.Services.AddSingleton<AiService>(sp =>
    new AiService(endpoint, apiKey));

var app = builder.Build();

app.MapPost("/api/character/generate", async (
    GenerateCharacterRequest request,
    AiService aiService) =>
{
    var result = await aiService.GenerateCharacter(request.Prompt);

    return Results.Ok(result);
});

app.Run();

public record GenerateCharacterRequest(string Prompt);