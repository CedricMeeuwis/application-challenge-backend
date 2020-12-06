using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Models
{
    public class Ploeg
    {
        public int PloegID { get; set; }
        public string Naam { get; set; }
        public string Bedrijfsnaam { get; set; }
        public string Locatie { get; set; }
        public string PloegFoto { get; set; }

        //Relations
        public int? KapiteinID { get; set; }
        public User? Kapitein { get; set; }

        [JsonIgnore]
        public ICollection<User> Leden { get; set; }
    }
}
