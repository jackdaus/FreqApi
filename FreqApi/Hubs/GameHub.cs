using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using FreqApi.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FreqApi.Hubs
{
    public class GameHub : Hub
    {
        private readonly FreqContext _context;

        public GameHub(FreqContext context)
        {
            _context = context;
        }

        public async Task JoinRoom(string roomCode, string username)
        {
            var game = _context.Games
                    .Include(game => game.Players)
                    .Where(game => game.RoomCode == roomCode && game.Phase == Phase.Registration)
                    .FirstOrDefault();
            if (game == null)
            {
                await Clients.Caller.SendAsync("errorRoomNotFound", roomCode);
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

                Player player = new Player();
                player.Id = Guid.NewGuid();
                player.Username = username;
                player.ConnectionId = Context.ConnectionId;

                _context.Players.Add(player); 
                game.Players.Add(player);

                await _context.SaveChangesAsync();
                await Clients.Caller.SendAsync("joinSuccess", game.Id);

                IEnumerable<String> playerNames = game.Players.Select(player => player.Username);

                await Clients.Group(roomCode).SendAsync("newUserJoined", playerNames);
            }
        }

        public async Task SubmitAxesIdea(Guid roomId, string leftWord, string rightWord)
        {
            //TODO add new Axes Idea
            //await Clients.All.SendAsync("messageReceived", username, message);
        }

        public async Task SubmitClue(Guid AxisId, string clue)
        {
            //TODO submit clues
            //await Clients.All.SendAsync("messageReceived", username, message);
        }

        public async Task SubmitAngleGuess(Guid roomId, Guid AxisId, int degree)
        {
            //TODO submit angle guesses
            //await Clients.All.SendAsync("messageReceived", username, message);
        }

        public async Task NewMessage(long username, string message)
        {
            await Clients.All.SendAsync("messageReceived", username, message);
        }
    }
}
