using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Models
{
    public class Challenge
    {
        public int ChallengeID { get; set; }
        public bool Geaccepteerd { get; set; }
        public bool Afgewezen { get; set; }

        //relations
        //uitdager
        public int? UitdagerUserID { get; set; }
        public User UitdagerUser { get; set; }

        //Ploeg van uitgedaagde
        public int? UitgedaagdePloegID { get; set; }
        public Ploeg UitgedaagdePloeg { get; set; }
            
    }
}
