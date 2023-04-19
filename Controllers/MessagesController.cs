using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingAgenda.Models;
using Microsoft.AspNetCore.SignalR;
using MeetingAgenda.Hubs;

namespace MeetingAgenda.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly DatabaseContext _context;

    private readonly IHubContext<MeetingHub> _hubt;

    public MessagesController(DatabaseContext context, IHubContext<MeetingHub> hubt)
    {
        _context = context;
        _hubt = hubt;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessageItems()
    {
        return await _context.Messages.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetMessageItem(int id)
    {
        var messageItem = await _context.Messages.FindAsync(id);

        if (messageItem == null)
        {
            return NotFound();
        }

        return messageItem;
    }

    [HttpPost]
    public async Task<ActionResult<Message>> PostMessageItem(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        await _hubt.Clients.All.SendAsync("MessageCreated", message);
        return CreatedAtAction(nameof(GetMessageItem), new { id = message.Id }, message);
    }

    //POST: api/Messages/1/Messages
    [HttpPost("{messageId}/Messages")]
    public async Task<Message> PostMeetingMessage(int meetingId,  Message Message)
    {
        Message.MeetingId = meetingId;
        _context.Messages.Add(Message);
        await _context.SaveChangesAsync();

        // return CreatedAtAction(nameof(GetMeetingItem), new { id = meeting.Id }, meeting);

        //broadcast the message to all clients listening to the meeting
        await _hubt.Clients.Group(meetingId.ToString()).SendAsync("ReceiveMessage", Message);
        return Message;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutMessageItem(int id, Message message)
    {
        if (id != message.Id)
        {
            return BadRequest();
        }

        _context.Entry(message).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        await _hubt.Clients.All.SendAsync("UpdateMessage", message);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessageItem(int id)
    {
        var messageItem = await _context.Messages.FindAsync(id);
        if (messageItem == null)
        {
            return NotFound();
        }

        _context.Messages.Remove(messageItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}