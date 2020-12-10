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
                new User { Email = "aa@test.be", Naam = "Andre Andermans",  Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot", PloegID = 1, IsKapitein = true },
                new User { Email = "bb@test.be", Naam = "Barry Barrens", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot", PloegID = 2, IsKapitein = true },
                new User { Email = "cc@test.be", Naam = "Connie Conners", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot", PloegID = 3, IsKapitein = true },
                new User { Email = "dd@test.be", Naam = "David Deckers", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot", PloegID = 4, IsKapitein = true },
                //Gebruikers
                new User { Email = "ee@test.be", Naam = "Erik Erens", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot", PloegID = 1 },
                new User { Email = "ff@test.be", Naam = "Femke Feyen", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot", PloegID = 2 },
                new User { Email = "gg@test.be", Naam = "Gerrie Gachternaam", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot", PloegID = 3 },
                new User { Email = "hh@test.be", Naam = "Harry Hekkens", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot", PloegID = 4 },
                //Testdata voor inloggen
                new User { Email = "kapitein@test.be", Naam = "Kapitein Test", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot", PloegID = 5, IsKapitein=true},
                new User { Email = "gebruiker@test.be", Naam = "Gebruiker Test", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot" },
                new User { Email = "admin@test.be", Naam = "Admin Test", Geboortedatum = DateTime.Now.AddDays(-7000), Passwoord = "o6w8D+OE1/nhuMhyD1iKz4JkqmDFH3Sk8s5kV1FnNPRah5ot", IsAdmin = true}

            );
            context.SaveChanges();

            context.Tafels.AddRange(
                new Tafel { Naam = "Tafel jstack", Bedrijfsnaam = "jstack", Adres = "Veldkant 33b, Kontich", ContactTelefoon = "04958536", ContactNaam = "Erik", ContactEmail = "ee@test.be" },
                new Tafel { Naam = "Tafel TM", Bedrijfsnaam = "Thomas more", Adres = "Kleinhoefstraat 4, Geel", ContactTelefoon = "04856921", ContactNaam = "David", ContactEmail = "aa@test.be" }
            );
            context.MatchContexten.AddRange(
                new MatchContext { },
                new MatchContext { TournooiID = 1, TournooiNiveau = 1, TournooiRangschikking = 5 }
            );

            context.Wedstrijden.AddRange(
                new Wedstrijd { Akkoord= true, Bezig = false, TafelID=1, Team1Score=10, Team2Score=6, Team1User1ID=1, Team1User2ID=5, Team2User1ID=2, Team2User2ID=6, MatchContextID= 1},
                new Wedstrijd { Akkoord = true, Bezig = false, TafelID = 2, Team1Score = 7, Team2Score = 10, Team1User1ID = 1, Team1User2ID = 5, Team2User1ID = 2, Team2User2ID = 6, MatchContextID = 2 }

            );
            context.Tournooien.AddRange(
                new Tournooi { Naam = "Jaarlijks Kampioenschap" }
                );
            context.SaveChanges();

        }
    }
}
