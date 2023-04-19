namespace MeetingAgenda.Models;
public class Message
{
    public Message(int id, string text, string userName, DateTime created)
    {
        Id = id;
        Text = text;
        UserName = userName;
        Created = created;
    }

    public int Id { get; set; }
    public string Text { get; set; } 
    public string UserName { get; set; }
    public DateTime Created { get; set; }
    public int MeetingId { get; set; }
    public Meeting? Meeting { get; set; }
}
