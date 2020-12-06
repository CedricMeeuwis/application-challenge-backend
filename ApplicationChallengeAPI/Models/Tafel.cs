using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Models
{
    public class Tafel
    {
        public int TafelID { get; set; }
        public string Naam { get; set; }
        public string Bedrijfsnaam { get; set; }
        public string Adres { get; set; }
        public string Foto { get; set; }

        public int ContactpersoonID { get; set; }
        public User Contactpersoon { get; set; }
    }
}
