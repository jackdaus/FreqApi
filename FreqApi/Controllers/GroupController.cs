using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreqApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FreqApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly FreqContext _context;

        public GroupController(FreqContext context)
        {
            _context = context;
        }

        [HttpGet("join")]
        public async Task<ActionResult<Game>> JoinGame(string roomCode, string username)
        {
            var currentGame = _context.Games.Where(game => game.RoomCode == roomCode).FirstOrDefault();
            if (currentGame == null || currentGame.Phase == Phase.Completed)
            {
                return NotFound();
            }
            
            if (currentGame.Phase != Phase.Registration)
            {
                return BadRequest("Game is in progress, you may not join.");
            }

            Player player = new Player();
            player.Id = Guid.NewGuid();
            player.Username = username;
            //player.GameId = currentGame.Id;

            _context.Players.Add(player);
            //_context.Entry(player).State = EntityState.Added;

            currentGame.Players.Add(player);

            await _context.SaveChangesAsync();

            return currentGame;
        }
    }
}
