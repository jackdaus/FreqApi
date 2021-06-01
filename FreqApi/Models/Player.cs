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
        [System.Text.Json.Serialization.JsonIgnore] // skip serialization for now. Once we use DTO, not needed
        public Guid GameId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore] // skip serialization for now. Once we use DTO, not needed
        public Game Game { get; set; }
    }
}
