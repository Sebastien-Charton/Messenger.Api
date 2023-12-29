using System.Security.Claims;
using Messenger.Api.Application.Message.Commands.RemoveUserConnectionId;
using Messenger.Api.Application.Message.Commands.SetUserConnectionId;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Messenger.Api.Web.Hubs;

[SignalRHub]
public class TestChatHub : Hub
{
    private readonly ISender _sender;
    public TestChatHub(ISender sender)
    {
        _sender = sender;
    }

    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;

        var setUserConnectionIdCommand = new SetUserConnectionIdCommand() { ConnectionId = connectionId };
        await _sender.Send(setUserConnectionIdCommand);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        var removeUserConnectionIdCommand = new RemoveUserConnectionIdCommand() { ConnectionId = connectionId };
        await _sender.Send(removeUserConnectionIdCommand);
    }
    public async Task SendMessage(string user, string message)
    {
        await Clients.Others.SendAsync("ReceiveMessage", user, message);
    }
    
    public async Task SendMessageToSpecificUser(string user, string message)
    {
        await Clients.User(user).SendAsync("ReceiveMessage", user, message);
    }
    
}
