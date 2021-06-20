using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using FreqApi.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FreqApi.Interfaces;

namespace FreqApi.Hubs
{
    public class GameHub : Hub
    {
        private readonly FreqContext _context;
        private readonly IGameService _gameService;


        public GameHub(FreqContext context, IGameService gameService)
        {
            _context = context;
            _gameService = gameService;
        }

        public async Task JoinRoom(string roomCode, string username)
        {
            var game = _context.Games
                    .Include(game => game.Players)
                    .Where(game => game.RoomCode == roomCode && game.Phase == Phase.Registration)
                    .FirstOrDefault();
            if (game == null)
            {
                await Clients.Caller.SendAsync("errorMsg", $"Room with code ${roomCode} not found");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

                Player player = new Player();
                player.Id = Guid.NewGuid();
                player.Username = username;
                player.ConnectionId = Context.ConnectionId;
                player.IsOwner = !game.Players.Any();
                player.Position = game.Players.Count() + 1;

                _context.Players.Add(player); 
                game.Players.Add(player);

                await _context.SaveChangesAsync();
                await Clients.Caller.SendAsync("joinSuccess", game.Id, player);

                var players = game.Players;

                await Clients.Group(roomCode).SendAsync("playerDataChange", players);
            }
        }

        public async Task ChangeName(string newUsername)
        {
            var callingConnectionId = Context.ConnectionId;
            var callingPlayer = _context.Players
                .Include(player => player.Game)
                .ThenInclude(game => game.Players)
                .Where(player => player.ConnectionId == callingConnectionId)
                .FirstOrDefault();

            if (callingPlayer == null)
            {
                await Clients.Caller.SendAsync("errorMsg", $"Player with connectionId ${callingConnectionId} does not exist.");
                return;
            }
            callingPlayer.Username = newUsername;
            await _context.SaveChangesAsync();

            var game = callingPlayer.Game;
            if (game != null)
            {
                var players = game.Players.OrderBy(player => player.Position);
                await Clients.Group(game.RoomCode).SendAsync("playerDataChange", players);
            }

        }

        public async Task StartGame(Guid gameId)
        {
            var game = _context.Games
                .Include(game => game.Players)
                .Where(game => game.Id == gameId)
                .FirstOrDefault();

            if (game == null)
            {
                await Clients.Caller.SendAsync("errorMsg", $"Game with id ${gameId} not found.");
                return;
            }
            if (game.Phase == Phase.Registration)
            {
                await Clients.Caller.SendAsync("errorMsg", $"Game with id ${gameId} is already in progress.");
                return;
            }

            var callingConnectionId = Context.ConnectionId;
            var callingPlayer = game.Players.Where(player => player.ConnectionId == callingConnectionId).FirstOrDefault();
            if (callingPlayer == null)
            {
                await Clients.Caller.SendAsync("errorMsg", $"You are not part of the game you are trying to start.");
                return;
            }
            if (!callingPlayer.IsOwner)
            {
                await Clients.Caller.SendAsync("errorMsg", $"You may not start this game. Only the game owner may start the game.");
                return;
            }

            this._gameService.StartGame(game.RoomCode);
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
