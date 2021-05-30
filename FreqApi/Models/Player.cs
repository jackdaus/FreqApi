using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreqApi.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public int Points { get; set; }
    }
}
