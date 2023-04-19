using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingAgenda.Models;


public class Meeting
{
    public Meeting(int id, string name, DateTime created)
    {
        Id = id;
        Name = name;
        Created = created;
        Messages = new List<Message>();
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Created { get; set; }
    public List<Message> Messages { get; set; }
}