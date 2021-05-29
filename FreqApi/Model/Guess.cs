using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreqApi.Model
{
    public class Guess
    {
        public Player Player { get; set; }
        public double Value { get; set; }
        public DateTime SubmissionTime { get; set; }
    }
}
