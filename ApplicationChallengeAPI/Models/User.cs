using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Naam { get; set; }
        public string Geboortedatum { get; set; }
        public string Foto { get; set; }
        public string Passwoord { get; set; }
        [NotMapped]
        public string Token { get; set; }

        //Relations
        public int PloegID { get; set; }
        public Ploeg Ploeg { get; set; }

        [JsonIgnore]
        public ICollection<Ploeg> Crews { get; set; }
    }
}
