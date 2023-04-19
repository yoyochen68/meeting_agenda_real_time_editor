using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingAgenda.Models;
using Microsoft.AspNetCore.SignalR;
using MeetingAgenda.Hubs;

namespace MeetingAgenda.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeetingsController : ControllerBase
{
    private readonly DatabaseContext _context;

    private readonly IHubContext<MeetingHub> _hubt;

    public MeetingsController(DatabaseContext context, IHubContext<MeetingHub> hubt)
    {
        _context = context;
        _hubt = hubt;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetingItems()
    {
        return await _context.Meetings.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Meeting>> GetMeetingItem(int id)
    {
        var meetingItem = await _context.Meetings.FindAsync(id);

        if (meetingItem == null)
        {
            return NotFound();
        }

        return meetingItem;
    }

    [HttpPost]
    public async Task<ActionResult<Meeting>> PostMeetingItem(Meeting meeting)
    {
        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();
        await _hubt.Clients.All.SendAsync("MeetingCreated", meeting);
        return CreatedAtAction(nameof(GetMeetingItem), new { id = meeting.Id }, meeting);
    }

//POST: api/Meetings/1/Messages
    [HttpPost("{meetingId}/Messages")]
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
    public async Task<IActionResult> PutMeetingItem(int id, Meeting meeting)
    {
        if (id != meeting.Id)
        {
            return BadRequest();
        }

        _context.Entry(meeting).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeetingItem(int id)
    {
        var meetingItem = await _context.Meetings.FindAsync(id);
        if (meetingItem == null)
        {
            return NotFound();
        }

        _context.Meetings.Remove(meetingItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}