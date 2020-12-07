using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Models
{
    public class Tournooi
    {
        public int TournooiID { get; set; }
        public string Naam { get; set; }

        [JsonIgnore]
        public ICollection<Competitie> Competities { get; set; }
    }
}
