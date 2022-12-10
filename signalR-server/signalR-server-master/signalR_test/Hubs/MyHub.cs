using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using signalR_server.Helper;
using signalR_server.Interfaces;
using signalR_server.Interfaces.Enums;
using signalR_server.Models;
using signalR_server.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace signalR_server.Hubs
{
    public class MyHub : Hub
    {
        public static List<User> clients = new List<User>();
        public static List<Group> groups = new List<Group>();
        public static List<DirectMessages> directMessages = new List<DirectMessages>();

  
        public async Task SendMessageAsync(string message)
        {
            User usr = UserHelper.FindUser(clients, Context.ConnectionId);
            
            await Clients.All.SendAsync("receiveMessage", message, Context.ConnectionId, usr.Username);
        }
        public async Task SendMessageToUserAsync(string message, string userName, string senderConnId)
        {
            User sender = new User();
            sender = UserHelper.FindUser(clients, senderConnId);
            User receiver = new User();
            receiver = UserHelper.FindUserByUsername(clients, userName);
            
            DirectMessages dm = new DirectMessages();
            dm = directMessages.Where(x => (x.userName1 == sender.Username || x.userName1 == receiver.Username)
                && (x.userName2 == sender.Username || x.userName2 == receiver.Username)).FirstOrDefault();

            dm.messages.Add(new DirectMessageResponse { senderUsername = sender.Username, directMessage = message });
           
            await Clients.Client(receiver.ConnectionId).SendAsync("receiveDirectMessage", message, receiver.ConnectionId, userName, sender.Username);
            await Clients.Caller.SendAsync("receiveDirectMessage", message, receiver.ConnectionId, userName, sender.Username);
        }

        public async Task SendMessageToGroupAsync(string message, string groupName)
        {

            GroupMessageResponse resp = new GroupMessageResponse();
            User user = UserHelper.FindUser(clients, Context.ConnectionId);
            resp = new GroupMessageResponse { message = message, connectionId = Context.ConnectionId, sender = user.Username, groupName = groupName };

            groups.Where(o => o.getGroupName() == groupName).FirstOrDefault().messages.Add(resp);
            await Clients.Group(groupName).SendAsync("receiveGroupMessage", JsonConvert.SerializeObject(resp));
        }

        public async Task GetPrevGroupMsgs(string groupName)
        {
            await Clients.Caller.SendAsync("receivePrevGroupMsgs", JsonConvert.SerializeObject(groups.Where(o => o.getGroupName() == groupName).FirstOrDefault().messages));
        }

        public async Task GetPrevUserMsgs(string userName, string myConnId)
        {
            User usr1 = UserHelper.FindUser(clients, myConnId);
            User usr2 = UserHelper.FindUserByUsername(clients, userName);
            DirectMessages dm = directMessages.Where(x => (x.userName1 == usr1.Username || x.userName1 == usr2.Username)
                && (x.userName2 == usr1.Username || x.userName2 == usr2.Username)).FirstOrDefault();
           
            if (dm == null)
            { /
                dm = new DirectMessages { userName1 = usr1.Username, userName2 = usr2.Username, messages = new List<DirectMessageResponse>() };
                directMessages.Add(dm);
            }
            await Clients.Caller.SendAsync("receivePrevUserMsgs", JsonConvert.SerializeObject(dm.messages));
        }

        public override async Task OnConnectedAsync()
        {
            
            await Clients.Caller.SendAsync("getConnectionId", Context.ConnectionId);
            clients.Add(new User(Context.ConnectionId)); 
            List<string> groupNames = new List<string>();
            foreach (Group grp in groups)
            {
                if (grp.getGroupName() != null)
                    groupNames.Add(grp.getGroupName());
            }
            await Clients.Caller.SendAsync("updateGroups", groupNames);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
    
            await Task.Delay(3000);

            User disconnectUser = clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            clients.Remove(disconnectUser); 
            List<string> userNames = new List<string>();

            userNames = clients.Where(o => o.Username != null).Select(o => o.Username).ToList();
          
            foreach (Group grp in groups)
            {
                User user = UserHelper.FindUser(grp.members, Context.ConnectionId);
                if (user.ConnectionId == Context.ConnectionId)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, grp.getGroupName());
                    grp.members.Remove(user);
                }
            }
            
            if (disconnectUser != null && disconnectUser.Username != null)
            {
                await Clients.All.SendAsync("userLeft", disconnectUser.Username);
                await Clients.All.SendAsync("clients", clients.Where(o => o.Username != null).Select(o => o.Username));
            }
        }


        public async Task AddGroup(string connectionId, string groupName)
        {
            var groupAlreadyExists = true;
            if (groups.Count >= (int)GroupEnum.maxGroupCount)
            {
                await Clients.All.SendAsync("groupLimitReached");
                return;
            }
  
            if (!GroupHelper.GroupExists(groups, groupName))
            {
                groupAlreadyExists = false;
                await Groups.AddToGroupAsync(connectionId, groupName);
                groups = GroupHelper.AddGroup(groups, groupName, clients, connectionId);
            }
            await Clients.All.SendAsync("checkAddGroup", groupAlreadyExists, groupName, connectionId);
        }

        public async Task JoinGroup(string connectionId, string groupName)
        {
            GroupResponse response = new GroupResponse();
            User usr = new User();
            var theGroup = groups.First();
            if (GroupHelper.GroupExists(groups, groupName)) 
            {
                usr = UserHelper.FindUser(clients, connectionId);
                theGroup = GroupHelper.FindGroup(groups, groupName);

                response = new GroupResponse{ClientId = connectionId, GroupName = groupName, members = theGroup.members, ClienInGroup = false};

                
                if (!UserHelper.UserExists(theGroup.members, usr))
                {
                    await Groups.AddToGroupAsync(connectionId, groupName);
                    theGroup.members.Add(usr);
                    response.ClienInGroup = true;
                }

            }

            await Clients.Caller.SendAsync("checkJoinGroup", JsonConvert.SerializeObject(response));
            await Clients.OthersInGroup(groupName).SendAsync("notificationJoinGroup", usr.Username);
        }
        public async Task LeaveGroup(string connectionId, string groupName)
        {
            User usr = new User();
            var theGroup = groups.First();
            if (GroupHelper.GroupExists(groups, groupName))
            {
                usr = UserHelper.FindUser(clients, connectionId);
                theGroup = GroupHelper.FindGroup(groups, groupName);
                if (UserHelper.UserExists(theGroup.members, usr))
                {
                    await Groups.RemoveFromGroupAsync(connectionId, groupName);
                    theGroup.members.Remove(usr);
                }

            }
            await Clients.Caller.SendAsync("checkLeaveGroup", groupName);
          
        }


        public async Task AddUserName(string userName, string connectionId)
        {
            var client = clients.FirstOrDefault(o => o.ConnectionId == connectionId);
            if (string.IsNullOrEmpty(userName) || client == null || clients.Where(o => o.Username == userName).Count() > 0)
            {
                await Clients.Caller.SendAsync("checkUserName", userName);
                return;
            };
            client.Username = userName;

            await Clients.Caller.SendAsync("userJoined", userName);
            await Clients.All.SendAsync("clients", clients.Where(o => o.Username != null).Select(o => o.Username));
            await Clients.Others.SendAsync("notifyUserJoined", userName);

        }

        public async Task RequestUserList()
        {
            await Clients.Caller.SendAsync("receiveUserList", JsonConvert.SerializeObject(clients));
        }

    }
}

