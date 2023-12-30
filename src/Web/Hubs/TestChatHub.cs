using System.Security.Claims;
using Messenger.Api.Application.Message.Commands.RemoveUserConnectionId;
using Messenger.Api.Application.Message.Commands.SetUserConnectionId;
using Messenger.Api.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Policy = Policies.AllUsers)]
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;

        var setUserConnectionIdCommand = new SetUserConnectionIdCommand() { ConnectionId = connectionId };
        await _sender.Send(setUserConnectionIdCommand);
    }
    
    [Authorize(Policy = Policies.AllUsers)]
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        var removeUserConnectionIdCommand = new RemoveUserConnectionIdCommand() { ConnectionId = connectionId };
        await _sender.Send(removeUserConnectionIdCommand);
    }
    
    [Authorize(Policy = Policies.AllUsers)]
    public async Task SendMessage(string user, string message)
    {
        await Clients.Others.SendAsync("ReceiveMessage", user, message);
    }
    
    [Authorize(Policy = Policies.AllUsers)]
    public async Task SendMessageToSpecificUser(string user, string message)
    {
        await Clients.User(user).SendAsync("ReceiveMessage", user, message);
    }
    
}
