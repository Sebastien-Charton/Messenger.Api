﻿using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Messenger.Api.Web.Hubs;

[SignalRHub]
public class TestChatHub : Hub
{
        public async Task SendMessage(string user, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }
}
