using FreqApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreqApi.Interfaces
{
    public interface IGameService
    {
        Game CreateNewGame();
        List<Game> GetActiveGames();
        void StartGame(string roomCode);
    }
}
