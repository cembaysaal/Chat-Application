using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalR_server.Interfaces
{
    public interface IMessageClient
    {
        Task Clients(List<string> clients);
        Task UserJoined(string connectionId);
        Task UserLeft(string connectionId);
    }
}
