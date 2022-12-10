using signalR_server.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalR_server.Models
{
    public class Group
    {
        private string groupName;
        private string createdByConnId;
        public List<User> members { get; set; } = new List<User>();
        public List<GroupMessageResponse> messages;

        public Group(string groupName, string createdByConnId)
        {
            this.groupName = groupName;
            this.createdByConnId = createdByConnId;
            messages = new List<GroupMessageResponse>();
            //members.Add(createdByConnId);
        }

        public string getGroupName()
        {
            return groupName;
        }

        public string getCreatedByConnId()
        {
            return createdByConnId;
        }

        public void setGroupName(string groupName)
        {
            this.groupName = groupName;
        }

        public void addMember(User user)
        {
            members.Add(user);
        }

        public void removeMember(User user)
        {
            members.Remove(user);
        }
    }
}
