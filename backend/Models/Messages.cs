namespace CharactherGenerator.Api.Models;

public class Message
{
    public int Id {get; set; }

    public int ChatId {get; set;}
    public Chat chat {get;set;} = null!;

    public string Role {get;set;} ="";
    public string Content {get;set;} ="";


    public DateTime CreatedAt {get; set;}
}