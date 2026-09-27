using System.ClientModel;
using OpenAI;
using OpenAI.Chat;
using CharactherGenerator.Api.Models;

namespace CharactherGenerator.Api.Services;

public class AiService
{
    private readonly ChatClient _client;

    public AiService(string endpoint, string key)
    {
        _client = new ChatClient(
            model: "gpt-4.1-mini",
            credential: new ApiKeyCredential(key),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri($"{endpoint.TrimEnd('/')}/openai/v1")
            }
        );
    }

    public async Task<string> GenerateResponse(IEnumerable<Message> history)
    {
        List<ChatMessage> messages =
        [
            new SystemChatMessage(
                "Du är en hjälpsam AI-assistent. " +
                "Kom ihåg kontexten från konversationen och svara naturligt på svenska."
            )
        ];

        foreach (var message in history)
        {
            if (message.Role == "user")
            {
                messages.Add(new UserChatMessage(message.Content));
            }
            else if (message.Role == "assistant")
            {
                messages.Add(new AssistantChatMessage(message.Content));
            }
        }

        ChatCompletion completion =
            await _client.CompleteChatAsync(messages);

        return completion.Content[0].Text;
    }
}