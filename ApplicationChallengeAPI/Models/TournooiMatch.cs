using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Models
{
    public class TournooiMatch
    {
        public int TournooiMatchID { get; set; }

        public int Niveau { get; set; }

        //Relations
        public int TournooiID { get; set; }
        public Tournooi Tournooi { get; set; }
        public int WedstrijdID { get; set; }
        public Wedstrijd Wedstrijd { get; set; }

    }
}
