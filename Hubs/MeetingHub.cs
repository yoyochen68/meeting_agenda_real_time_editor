using Microsoft.AspNetCore.SignalR;
using MeetingAgenda.Models;

namespace MeetingAgenda.Hubs;
public class MeetingHub : Hub
{
    public async void SendMessage(Message message)
    {
        Console.WriteLine($"Received message: {message}");
         await Clients.All.SendAsync("ReceiveMessage", message);
    }
    public async Task UpdateMessage(Message message)
    {
        Console.WriteLine($"Updated message: {message}");
        await Clients.All.SendAsync("UpdateMessage", message);
    }


    public override Task OnConnectedAsync()
    {
        Console.WriteLine("A Client Connected: " + Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("A client disconnected: " + Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task AddToGroup(string roomId, string userName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        Console.WriteLine($"{userName} has joined the room {roomId}.");
        await Clients.Group(roomId).SendAsync("Joined",new { roomName = roomId, userName = userName });
    }

    public async Task RemoveFromGroup(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        

        await Clients.Group(roomName).SendAsync("Left", $"{Context.ConnectionId} has left the room {roomName}.");
    }




}