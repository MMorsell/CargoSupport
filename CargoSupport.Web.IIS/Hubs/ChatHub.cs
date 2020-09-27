using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CargoSupport.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendUpsertSignal(string id)
        {
            await Clients.All.SendAsync("Upsert", id);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}