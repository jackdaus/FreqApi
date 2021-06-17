using FreqApi.Hubs;
using FreqApi.Interfaces;
using FreqApi.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreqApi.Services
{
    public class GameService : IGameService
    {
        private readonly IHubContext<GameHub> _hubContext;
        private List<ActiveGame> activeGames;

        public GameService(IHubContext<GameHub> hubContext)
        {
            this._hubContext = hubContext;
            this.activeGames = new List<ActiveGame>();
        }
        public Game CreateNewGame()
        {
            var newGame = new Game();
            newGame.Id = Guid.NewGuid();
            newGame.Phase = Phase.Registration;
            newGame.CreationDate = DateTime.Now;
            newGame.RoomCode = newGame.Id.ToString().Substring(0, 4); //TODO ensure room code is unique...

            var newActiveGame = new ActiveGame();
            newActiveGame.Game = newGame;

            this.activeGames.Add(newActiveGame);

            return newGame;
        }

        public List<Game> GetActiveGames()
        {
            return this.activeGames.Select(ag => ag.Game).ToList();
        }
        public void StartGame(string roomCode)
        {
            var activeGame = this.activeGames.Find(activeGame => activeGame.Game.RoomCode == roomCode);
            if (activeGame == null)
            {
                throw new Exception($"Game not found with roomCode ${roomCode}");
            }

            if (activeGame.Game.Phase != Phase.Registration)
            {
                throw new Exception($"Game has already started");
            }

            this._hubContext.Clients.Group(roomCode).SendAsync("gameStarted");
            activeGame.Game.Phase = Phase.Ideation;

            // Set timer for 10 seconds. After 10 seconds, the next phase is triggered
            activeGame.Timer = new System.Timers.Timer(10000);
            activeGame.Timer.Elapsed += (sender, args) => BeginClueGivingPhase(activeGame);
            activeGame.Timer.Enabled = true;
            activeGame.Timer.AutoReset = false;

            //GC.KeepAlive needed?
        }

        private void BeginClueGivingPhase(ActiveGame activeGame)
        {
            var roomCode = activeGame.Game.RoomCode;
            activeGame.Game.Phase = Phase.ClueGiving;

            this._hubContext.Clients.Group(roomCode).SendAsync("beginClueGivingPhase");
            Console.WriteLine("call back triggered for roomCode "  + roomCode);
        }

    }
}
