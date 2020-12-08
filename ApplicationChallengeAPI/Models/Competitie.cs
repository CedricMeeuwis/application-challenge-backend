using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Models
{
    public class Competitie
    {
        public int CompetitieID { get; set; }
        public string Periode { get; set; }
        public int ParticipentAantal { get; set; }//1 = 1v1; 2 = 2v2

        [JsonIgnore]
        public ICollection<MatchContext> MatchContexten { get; set; }
    }
}
