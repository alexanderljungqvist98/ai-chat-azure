using CharactherGenerator.Api.Services;
using CharactherGenerator.Api.Data;
using CharactherGenerator.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(Options =>
Options.UseSqlite("Data Source=characther-generator.db"));

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

app.MapPost("/api/chats", async (AppDbContext db) =>
{
    var chat = new Chat
    {
        Title = "Ny chatt",
        CreatedAt = DateTime.UtcNow
    };

    db.Chats.Add(chat);
    await db.SaveChangesAsync();

    return Results.Ok(chat);
});

app.MapGet("/api/chats", async (AppDbContext db) =>
{
    var chats = await db.Chats
    .OrderByDescending(chat => chat.CreatedAt)
    .ToListAsync();

    return Results.Ok(chats);

});


app.Run();

public record GenerateCharacterRequest(string Prompt);