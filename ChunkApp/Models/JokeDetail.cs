using System;
using System.Collections.Generic;

#nullable disable

namespace ChunkApp.Models
{
    public partial class JokeDetail
    {
        public int Id { get; set; }
        public string Joke { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public int JokeCount { get; set; }
        public string OriginalId { get; set; }
    }
}
