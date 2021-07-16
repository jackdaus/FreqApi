using FreqApi.Hubs;
using FreqApi.Interfaces;
using FreqApi.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreqApi.Services
{
    public class GameService : IGameService
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private List<ActiveGame> activeGames;

        public GameService(IHubContext<GameHub> hubContext, IServiceScopeFactory scopeFactory)
        {
            this._hubContext = hubContext;
            this._scopeFactory = scopeFactory;
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

            activeGame.Game.Phase = Phase.AxisIdeation;
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FreqContext>();
                var currentGame = dbContext.Games.Find(activeGame.Game.Id);
                currentGame.Phase = Phase.AxisIdeation;
                dbContext.SaveChanges();
            }

            this._hubContext.Clients.Group(roomCode).SendAsync("gameStarted", activeGame.Game);

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
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FreqContext>();
                var currentGame = dbContext.Games.Find(activeGame.Game.Id);
                currentGame.Phase = Phase.ClueGiving;

                var currentGameAxes = dbContext.Axes.Where(axis => axis.GameId == activeGame.Game.Id).ToList();

                // Assign each player to an axis. We will use the axis created by the "player to the right" (if all players were sitting in a circle)
                var players = dbContext.Players.Where(player => player.GameId == activeGame.Game.Id).ToArray();
                for (int i = 0; i < players.Length; i++)
                {
                    var player = players[i];
                    var playerToRight = players[(i + 1) % players.Length];
                    var assignedAxis = currentGameAxes.Where(axis => axis.AxisAuthorId == playerToRight.Id).FirstOrDefault();

                    if (assignedAxis != null)
                    {
                        assignedAxis.ClueAuthorId = player.Id;
                    } 
                    else
                    {
                        assignedAxis = new Axis();
                        assignedAxis.GameId = activeGame.Game.Id;
                        //TODO make a collection of predined default axes to use
                        assignedAxis.LeftWord = "Cold";
                        assignedAxis.RightWord = "Hot";
                        assignedAxis.ClueAuthorId = player.Id;
                        dbContext.Axes.Add(assignedAxis);
                    }

                    assignedAxis.TargetNumber = new Random().Next(0, 180);

                    this._hubContext.Clients.Client(player.ConnectionId).SendAsync("cluePhase", assignedAxis);
                }

                dbContext.SaveChanges();
            }


            this._hubContext.Clients.Group(roomCode).SendAsync("phaseChange", activeGame.Game);
        }

    }
}
