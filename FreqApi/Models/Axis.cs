using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FreqApi.Models
{
    public class Axis
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        [ForeignKey("Player")]
        public Guid? AxisAuthorId { get; set; }
        public string LeftWord { get; set; }
        public string RightWord { get; set; }
        public int TargetNumber { get; set; }
        public string Clue { get; set; }
        [ForeignKey("Player")]
        public Guid? ClueAuthorId { get; set; }

        public ICollection<Guess> Guesses { get; set; }
    }
}
