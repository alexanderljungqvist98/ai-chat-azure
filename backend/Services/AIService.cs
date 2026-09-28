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
                """
                Du är en hjälpsam och vänlig AI-assistent.

                Du ska:
                - Svara alltid på svenska.
                - Vara tydlig, naturlig och hjälpsam.
                - Använda information från tidigare meddelanden i konversationen.
                - Hålla dig till ämnet användaren frågar om.
                - Ställa följdfrågor om information saknas.
                - Hjälpa användaren att utveckla idéer och lösa problem.
                - Inte påstå att du vet något som inte finns i konversationen.

                Du är en del av en chattapplikation där varje konversation
                kan innehålla flera meddelanden. Använd därför tidigare meddelanden
                som kontext när du svarar.

                Om användaren ber om hjälp med en rollspelskaraktär ska du kunna
                hjälpa till med exempelvis namn, personlighet, bakgrund, färdigheter,
                utrustning och andra egenskaper.

                Anpassa dina svar efter användarens frågor och tidigare konversation.
                """
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