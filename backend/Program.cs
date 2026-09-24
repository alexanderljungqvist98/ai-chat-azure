using CharactherGenerator.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("React", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var endpoint = builder.Configuration["AzureAI:Endpoint"]
    ?? throw new InvalidOperationException("AzureAI:Endpoint saknas.");

var apiKey = builder.Configuration["AzureAI:ApiKey"]
    ?? throw new InvalidOperationException("AzureAI:ApiKey saknas.");

builder.Services.AddSingleton<AiService>(sp =>
    new AiService(endpoint, apiKey));

var app = builder.Build();

app.UseCors("React");

app.MapPost("/api/character/generate", async (
    GenerateCharacterRequest request,
    AiService aiService) =>
{
    var result = await aiService.GenerateCharacter(request.Prompt);

    return Results.Ok(result);
});

app.Run();

public record GenerateCharacterRequest(string Prompt);