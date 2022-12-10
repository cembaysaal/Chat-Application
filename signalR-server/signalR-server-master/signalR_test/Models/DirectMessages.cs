using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalR_server.Models
{
    public class DirectMessages
    {
        public string userName1;
        public string userName2;
        public List<DirectMessageResponse> messages;

    }
}
