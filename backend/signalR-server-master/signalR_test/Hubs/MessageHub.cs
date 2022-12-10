using Microsoft.AspNetCore.SignalR;
using signalR_server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalR_server.Hubs
{
    public class MessageHub : Hub
    {
        //public async Task SendMessageAsync(string message)
        //public async Task SendMessageAsync(string message, IReadOnlyList<string> connectionIds)
        public async Task SendMessageAsync(string message, string groupName)
        {
            #region Caller
            // only communicates with the client that sent the message
            // await Clients.Caller.SendAsync("receiveMessage", message);
            #endregion

            #region All
            // communicates with all the clients connected to the server
            //await Clients.All.SendAsync("receiveMessage", message);
            #endregion

            #region Others
            // communicates with all the clients connected to the server
            // except the one who sent the message
            //await Clients.Others.SendAsync("receiveMessage", message);
            #endregion

            #region AllExcept
            // communicates with all the clients connected to the server
            // except the given connectionIds 
            //await Clients.AllExcept(connectionIds).SendAsync("receiveMessage", message);
            #endregion

            #region Client
            // communicates with the client with the given connectionId 
            //await Clients.Client(connectionIds[0]).SendAsync("receiveMessage", message);
            #endregion

            #region Clients
            // communicates with all the clients in the given connectionIds
            //await Clients.Clients(connectionIds).SendAsync("receiveMessage", message);
            #endregion

            #region Group
            // communicates with all the clients in the group
            // but the clients should subscribe to the group
            await Clients.Group(groupName).SendAsync("receiveMessage", message);
            #endregion

            #region GroupExcept
            // communicates with all the clients in the group
            // except for the ones with given connectionIds
            //await Clients.GroupExcept(groupName, connectionIds).SendAsync("receiveMessage", message);
            #endregion

        }


        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("getConnectionId", Context.ConnectionId);
        }

        public async Task AddGroup(string connectionId, string groupName)
        {
            // adds the client to the group 
            // if the group doesn't exist it creates the group first 
            await Groups.AddToGroupAsync(connectionId, groupName);
        }
    }
}
