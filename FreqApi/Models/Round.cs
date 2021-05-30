using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreqApi.Models
{
    public class Round
    {
        public Player ClueGiver { get; set; }
        public string Clue { get; set; }
        public IEnumerable<Guess> Guesses { get; set; }
    }
}
