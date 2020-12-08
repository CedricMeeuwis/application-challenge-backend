using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Models
{
    public class MatchContext
    {
        public int MatchContextID { get; set; }

        public int? TournooiNiveau { get; set; }
        public int? TournooiRangschikking { get; set; }

        //Relations
        public int WedstrijdID { get; set; }
        public Wedstrijd Wedstrijd { get; set; }
        public int? TournooiID { get; set; }
        public Tournooi Tournooi { get; set; }
        public int? CompetitieID { get; set; }
        public Competitie Competitie { get; set; }

    }
}
