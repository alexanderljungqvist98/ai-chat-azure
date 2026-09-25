using OpenAI.Assistants;


namespace CharactherGenerator.Api.Models;


public class Chat
{
    public int Id {get; set;}

    public string Title {get; set; } = "";

    public DateTime CreatedAt {get; set;}

    public List<Message> Messages { get; set; } = new();
}