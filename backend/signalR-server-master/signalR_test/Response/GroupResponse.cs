using signalR_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalR_server.Response
{
    public class GroupResponse
    {
        public string ClientId { get; set; }
        public string GroupName { get; set; }
        public bool ClienInGroup{ get; set; }
        public List<User> members{ get; set; }

    }
}
