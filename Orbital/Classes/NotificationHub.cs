using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Orbital.Classes
{
    public class NotificationHub : Hub
    {

        public override Task OnConnectedAsync()
        {
            Debug.WriteLine(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

    }
}
