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
        public DateTime Geboortedatum { get; set; }
        public string Foto { get; set; }
        public string Passwoord { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsKapitein { get; set; }
        [NotMapped]
        public string Token { get; set; }

        //Relations
        public int? PloegID { get; set; }
        public Ploeg Ploeg { get; set; }
    }
}
