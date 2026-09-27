using CharactherGenerator.Api.Services;
using CharactherGenerator.Api.Data;
using CharactherGenerator.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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

app.MapPost("/api/chats/{chatId}/messages", async (
    int chatId,
    SendMessageRequest request,
    AppDbContext db,
    AiService aiService) =>
{
    var chat = await db.Chats.FindAsync(chatId);

    if (chat is null)
    {
        return Results.NotFound();
    }

    var hasPreviousUserMessage = await db.Messages
        .AnyAsync(message =>
            message.ChatId == chatId &&
            message.Role == "user");

    var userMessage = new Message
    {
        ChatId = chatId,
        Role = "user",
        Content = request.Content,
        CreatedAt = DateTime.UtcNow
    };

    // Skapa automatiskt chattnamn från första meddelandet
    if (!hasPreviousUserMessage)
    {
        var title = request.Content.Trim();

        if (title.Length > 35)
        {
            title = title[..35] + "...";
        }

        chat.Title = title;
    }

    db.Messages.Add(userMessage);

    await db.SaveChangesAsync();

    var history = await db.Messages
        .Where(message => message.ChatId == chatId)
        .OrderBy(message => message.CreatedAt)
        .ToListAsync();

    var aiResponse = await aiService.GenerateResponse(history);

    var assistantMessage = new Message
    {
        ChatId = chatId,
        Role = "assistant",
        Content = aiResponse,
        CreatedAt = DateTime.UtcNow
    };

    db.Messages.Add(assistantMessage);
    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        Chat = new
        {
            chat.Id,
            chat.Title,
            chat.CreatedAt
        },

        UserMessage = new
        {
            userMessage.Id,
            userMessage.ChatId,
            userMessage.Role,
            userMessage.Content,
            userMessage.CreatedAt
        },

        AssistantMessage = new
        {
            assistantMessage.Id,
            assistantMessage.ChatId,
            assistantMessage.Role,
            assistantMessage.Content,
            assistantMessage.CreatedAt
        }
    });
});

app.MapGet("/api/chats/{chatId}/messages", async (
    int chatId,
    AppDbContext db) =>
{
    var messages = await db.Messages
        .Where(message => message.ChatId == chatId)
        .OrderBy(message => message.CreatedAt)
        .Select(message => new
        {
            message.Id,
            message.ChatId,
            message.Role,
            message.Content,
            message.CreatedAt
        })
        .ToListAsync();

    return Results.Ok(messages);
});

app.MapDelete("/api/chats/{chatId}", async (
    int chatId,
    AppDbContext db) =>
{
    var chat = await db.Chats.FindAsync(chatId);

    if (chat is null)
    {
        return Results.NotFound();
    }

    db.Chats.Remove(chat);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

