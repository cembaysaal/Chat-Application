using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using signalR_server.Business;
using signalR_server.Hubs;

namespace signalR_server.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // you can use either myBusiness or hubContext
        readonly MyBusiness myBusiness;
        readonly IHubContext<MyHub> hubContext;
        public HomeController(MyBusiness myBusiness, IHubContext<MyHub> hubContext)
        {
            this.myBusiness = myBusiness;
            this.hubContext = hubContext;
        }

        public async Task<IActionResult> Index(string message)
        {
            await myBusiness.SendMessageAsync(message);
            return Ok();
        }
    }
}