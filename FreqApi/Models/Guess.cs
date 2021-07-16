using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreqApi.Models
{
    public class Guess
    {
        public Guid Id { get; set; }
        public Guid AxisId { get; set; }
        public Player Player { get; set; }
        public double Value { get; set; }
        public DateTime SubmissionTime { get; set; }
    }
}
