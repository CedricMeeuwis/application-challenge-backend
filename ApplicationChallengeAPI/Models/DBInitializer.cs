using ApplicationChallengeAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Models
{
    public class DBInitializer
    {
        public static void Initialize(ChallengeContext context)
        {
            context.Database.EnsureCreated();

            // Look for any user.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }
            //To be done
            context.Ploegen.AddRange(
              new Ploeg { Naam = "Jstack United", Bedrijfsnaam = "jstack", Locatie = "Kontich" },
              new Ploeg { Naam = "Grass City", Bedrijfsnaam = "Grasshoppers", Locatie = "Kontich" },
              new Ploeg { Naam = "QF", Bedrijfsnaam = "Qframe", Locatie = "Kontich" },
              new Ploeg { Naam = "More Geel", Bedrijfsnaam = "Thomas more", Locatie = "Geel" },
              new Ploeg { Naam = "Testploeg", Bedrijfsnaam = "Testers", Locatie = "Testeren"}
            );
            context.SaveChanges();

            context.Users.AddRange(
                //Kapiteinen
                new User { Email = "aa@test.be", Naam = "Andre Andermans",  Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test", PloegID = 1, IsKapitein = true },
                new User { Email = "bb@test.be", Naam = "Barry Barrens", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test", PloegID = 2, IsKapitein = true },
                new User { Email = "cc@test.be", Naam = "Connie Conners", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test", PloegID = 3, IsKapitein = true },
                new User { Email = "dd@test.be", Naam = "David Deckers", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test", PloegID = 4, IsKapitein = true },
                //Gebruikers
                new User { Email = "ee@test.be", Naam = "Erik Erens", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test", PloegID = 1 },
                new User { Email = "ff@test.be", Naam = "Femke Feyen", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test", PloegID = 2 },
                new User { Email = "gg@test.be", Naam = "Gerrie Gachternaam", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test", PloegID = 3 },
                new User { Email = "hh@test.be", Naam = "Harry Hekkens", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test", PloegID = 4 },
                //Testdata voor inloggen
                new User { Email = "kapitein@test.be", Naam = "Kapitein Test", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test" , PloegID = 5, IsKapitein=true},
                new User { Email = "gebruiker@test.be", Naam = "Gebruiker Test", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test"},
                new User { Email = "admin@test.be", Naam = "Admin Test", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "test", IsAdmin = true}

            );
            context.SaveChanges();

            context.Tafels.AddRange(
                new Tafel { Naam = "Tafel jstack", Bedrijfsnaam = "jstack", Adres = "Veldkant 33b, Kontich", ContactpersoonID = 1 },
                new Tafel { Naam = "Tafel TM", Bedrijfsnaam = "Thomas more", Adres = "Kleinhoefstraat 4, Geel", ContactpersoonID = 4 }
            );
            context.SaveChanges();
        }
    }
}
