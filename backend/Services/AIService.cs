using System.ClientModel;
using OpenAI;
using OpenAI.Chat;

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

    public async Task<string> GenerateCharacter(string prompt)
    {
        ChatCompletion response = await _client.CompleteChatAsync(prompt);

        return response.Content[0].Text;
    }
}