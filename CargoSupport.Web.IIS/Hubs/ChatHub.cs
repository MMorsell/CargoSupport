using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CargoSupport.Hubs
{
    /// <summary>
    /// The SignalR connection hub to send updates between clients and the host
    /// </summary>
    public class ChatHub : Hub
    {
        /// <summary>
        /// Sends sigal to all users to reload datatable
        /// </summary>
        /// <returns>Promise of the Task</returns>
        public async Task ReloadDataTable()
        {
            await Clients.All.SendAsync("ReloadDataTable");
        }

        public async Task SendUpsertSignal(string id)
        {
            await Clients.All.SendAsync("Upsert", id);
        }

        /// <summary>
        /// Returns the connection Id
        /// </summary>
        /// <returns>The connection Id as a string</returns>
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}