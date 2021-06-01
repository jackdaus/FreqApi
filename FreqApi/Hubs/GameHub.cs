using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using FreqApi.Models;
using System.Linq;


namespace FreqApi.Hubs
{
    public class GameHub : Hub
    {
        private readonly FreqContext _context;

        public GameHub(FreqContext context)
        {
            _context = context;
        }

        public async Task JoinGame(string roomCode, string username)
        {
            var game = _context.Games.Where(game => game.RoomCode == roomCode);
            if (game == null)
            {

            }
            await Clients.Group(roomCode).SendAsync("newUserJoined", username);
        }

        public async Task NewMessage(long username, string message)
        {
            await Clients.All.SendAsync("messageReceived", username, message);
        }
    }
}
