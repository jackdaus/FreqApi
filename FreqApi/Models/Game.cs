using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreqApi.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public string RoomCode { get; set; }
        public Phase Phase { get; set; }
        public DateTime CreationDate { get; set; }

        public ICollection<Player> Players { get; set; }
    }

    public class ActiveGame
    {
        public Game Game { get; set; }
        public System.Timers.Timer Timer { get; set; }
    }

    public enum Phase
    {
        Registration,
        Ideation,
        ClueGiving,
        Guessing,
        RoundResults,
        GameResults,
        Completed
    }
}
