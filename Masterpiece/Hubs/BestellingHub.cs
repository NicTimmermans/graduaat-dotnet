using Microsoft.AspNetCore.SignalR;

namespace Restaurant.Hubs
{
    public class BestellingHub : Hub
    {
        // Methode kan vanuit controller worden aangeroepen
        public async Task SendUpdate()
        {
            await Clients.All.SendAsync("ReceiveUpdate");
        }
    }
}
