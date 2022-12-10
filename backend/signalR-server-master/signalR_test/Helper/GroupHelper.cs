using System;
using System.Collections.Generic;
using System.Linq;
using signalR_server.Models;

namespace signalR_server.Helper
{
    public static class GroupHelper
    {       
        public static bool GroupExists(List<Group> groups, string groupName)
        { 
            return groups.Where(o => o.getGroupName() == groupName).Any(); 
        }

        public static List<Group> AddGroup(List<Group> groups, string groupName, List<User> clients, string connectionId)
        {
            User user = UserHelper.FindUser(clients, connectionId);
            Group newGroup = new Group(groupName, connectionId);
            newGroup.addMember(user);
            groups.Add(newGroup);
            return groups;
        }

        public static Group FindGroup(List<Group> groups, string groupName)
        { 
            return groups.Where(o => o.getGroupName() == groupName).FirstOrDefault(); 
        }

       
    }
}
