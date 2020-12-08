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

        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public bool Bezig { get; set; }
        public bool Akkoord { get; set; }
        //Relations

        //Team 1 spelers
        public int? Team1User1ID { get; set; }
        public User Team1User1 { get; set; }
        //optioneel voor 1v1
        public int? Team1User2ID { get; set; }
        public User Team1User2 { get; set; }

        //Team 2 spelers
        public int? Team2User1ID { get; set; }
        public User Team2User1 { get; set; }
        //optioneel voor 1v1
        public int? Team2User2ID { get; set; }
        public User Team2User2 { get; set; }
    }
}
