using signalR_server.Hubs;
using signalR_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalR_server.Helper
{
    public static class UserHelper
    {
        public static User FindUser(List<User> clients, string connectionId)
        { 
            return clients.Where(x => x.ConnectionId == connectionId).FirstOrDefault(); 
        }
        public static User FindUserByUsername(List<User> clients, string userName)
        { 
            return clients.Where(x => x.Username == userName).FirstOrDefault();
        }
        public static bool UserExists(List<User> users, User usr)
        {
            return users.Contains(usr); 
        }

       
    }
}
