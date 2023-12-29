using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Messenger.Api.Web.Hubs;

[SignalRHub]
public class TestChatHub : Hub
{
    // public override async Task OnConnectedAsync()
    // {
    //     //get the connection id
    //     var connectionid = Context.ConnectionId;
    //     //get the username or userid
    //     var username = Context.User.Identity.Name;
    //     var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
    //
    //     //insert or updatde them into database.
    //     var CId = _context.UserIdToCId.Find(userId);
    //     CId.ConnectionId = connectionid;
    //     _context.Update(CId);
    //     await _context.SaveChangesAsync();
    //     await base.OnConnectedAsync();
    // }
    //
    // public override async Task OnDisconnectedAsync(Exception exception)
    // {
    //     //get the connection id
    //     var connectionid = Context.ConnectionId;
    //     //get the username or userid
    //     var username = Context.User.Identity.Name;
    //     var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
    //
    //     //insert or updatde them into database.
    //     var CId = _context.UserIdToCId.Find(userId);
    //     CId.ConnectionId = connectionid;
    //     _context.Update(CId);
    //     await _context.SaveChangesAsync();
    //     await base.OnDisconnectedAsync(exception);
    // }
    public async Task SendMessage(string user, string message)
    {
        await Clients.Others.SendAsync("ReceiveMessage", user, message);
    }
    
    public async Task SendMessageToSpecificUser(string user, string message)
    {
        await Clients.User(user).SendAsync("ReceiveMessage", user, message);
    }
    
}
