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
              new Ploeg { }
              );
            context.Tafels.AddRange(
                new Tafel {  }
              );

            context.Users.AddRange(
                new User { }
                );
            //context.SaveChanges();
        }
    }
}
