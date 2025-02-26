using Microsoft.AspNetCore.SignalR;

namespace hydrotronics_api
{
    public class DeviceHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("DeviceUpdated", message);
        }
    }
}

