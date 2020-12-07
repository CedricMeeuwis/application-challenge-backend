using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Models
{
    public class Wedstrijd
    {
        public int WedstrijdID { get; set; }

        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public bool Bezig { get; set; }

        //Relations

        //Home spelers
        public int HomeUser1ID { get; set; }
        public User HomeUser1 { get; set; }
        //optioneel voor 1v1
        public int? HomeUser2ID { get; set; }
        public User HomeUser2 { get; set; }

        //Away spelers
        public int AwayUser1ID { get; set; }
        public User AwayUser1 { get; set; }
        //optioneel voor 1v1
        public int? AwayUser2ID { get; set; }
        public User AwayUser2 { get; set; }
    }
}
