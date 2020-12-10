using ApplicationChallengeAPI.Data;
using ApplicationChallengeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private readonly ChallengeContext _context;

        public ChallengeController(ChallengeContext context)
        {
            _context = context;
        }

        // GET: api/Challenge
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Challenge>>> GetChallenges()
        {
            return await _context.Challenges.ToListAsync();
        }

        // GET: api/Challenge/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Challenge>> GetChallenge(int id)
        {
            var challenge = await _context.Challenges.FindAsync(id);

            if (challenge == null)
            {
                return NotFound();
            }

            return challenge;
        }

        // GET: api/Challenge/PloegChallenges
        [Authorize]
        [HttpGet("PloegChallenges")]
        public async Task<ActionResult<IEnumerable<Challenge>>> GetChallengesPerTeam()
        {
            //Check of user wel een ploeg heeft
            if(string.IsNullOrWhiteSpace(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value))
            {
                return NotFound("Je zit niet in een ploeg");
            }
            int ploegID = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value);

            var challenges = await _context.Challenges
                .Include(c => c.UitdagerPloeg).Include(c => c.UitgedaagdePloeg).Include(c => c.Wedstrijd)
                .Where(c => c.UitdagerPloegID == ploegID || c.UitgedaagdePloegID == ploegID)
                .Where(c => c.Geaccepteerd == true || c.Afgewezen == true)
                .OrderByDescending(c => c.ChallengeID)
                .ToListAsync();

            return challenges;
        }

        // GET: api/Challenge/PloegOpenChallenges
        [Authorize]
        [HttpGet("PloegOpenChallenges")]
        public async Task<ActionResult<IEnumerable<Challenge>>> GetOpenChallengesPerTeam()
        {
            //Check of user wel een ploeg heeft
            if (string.IsNullOrWhiteSpace(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value))
            {
                return NotFound("Je zit niet in een ploeg");
            }
            int ploegID = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value);

            var challenges = await _context.Challenges
                .Include(c => c.UitdagerPloeg).Include(c => c.UitgedaagdePloeg).Include(c => c.Wedstrijd)
                .Where(c => c.UitdagerPloegID == ploegID || c.UitgedaagdePloegID == ploegID)
                .Where(c => c.Geaccepteerd == false && c.Afgewezen == false)
                .OrderByDescending(c => c.ChallengeID)
                .ToListAsync();

            return challenges;
        }

        // PUT: api/Challenge/Reactie/5
        [Authorize]
        [HttpPut("Reactie/{id}")]
        public async Task<IActionResult> ReactieChallenge(int id, bool geaccepteerd)
        {
            if(string.IsNullOrWhiteSpace(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value))
            {
                return Unauthorized();
            }
            int ploegID = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value);
            int userID = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserID").Value);

            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge == null)
            {
                return NotFound();
            }

            if (ploegID != challenge.UitgedaagdePloegID)
            {
                return Unauthorized();
            }

            //Indien aanvaardChallenge true is, zet de challenge op accepted
            if(geaccepteerd)
            {
                challenge.Geaccepteerd = true;
                //Maak een wedstrijd aan
                Wedstrijd challengeWedstrijd = new Wedstrijd
                {
                    Team1User1ID = challenge.UitdagerUserID,
                    Team2User1ID = userID
                };

                _context.Wedstrijden.Add(challengeWedstrijd);
                await _context.SaveChangesAsync();
                //link wedstrijd aan de challenge
                challenge.WedstrijdID = challengeWedstrijd.WedstrijdID;
            }
            //Afwijzen van challenge
            else
            {
                challenge.Afgewezen = true;
            }
            _context.Entry(challenge).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChallengeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Challenge
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Challenge>> PostChallenge(Challenge challenge)
        {
            //Check of de uitgegaade ploeg bestaat
            var ploeg = await _context.Ploegen.FindAsync(challenge.UitgedaagdePloegID);

            if (ploeg == null)
            {
                return NotFound();
            }

            //Check of de uitdager wel in een team zit
            if(string.IsNullOrWhiteSpace(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value)) {
                return Unauthorized();
            }

            int ploegID = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value);

            //Check of de ploeg geen openstaande challenge heeft
            var openChallenge = await _context.Challenges
                .Where(c => c.Geaccepteerd == false && c.Afgewezen == false)
                .FirstOrDefaultAsync(c => c.UitdagerPloegID == ploegID);

            if(openChallenge != null)
            {
                return Conflict("Je ploeg heeft al een openstaande challenge");
            }

            //Check of de gebruiker niet zijn eigen ploeg uitdaagd
            if (ploegID == challenge.UitgedaagdePloegID)
            {
                return Conflict("Je kan je eigen ploeg niet uitdagen");
            }

            int userID = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserID").Value);
            //Zet ploeg en ID van gebruiker in challenge
            challenge.UitdagerPloegID = ploegID;
            challenge.UitdagerUserID = userID;
            challenge.WedstrijdID = null;
            _context.Challenges.Add(challenge);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ChallengeExists(challenge.ChallengeID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetChallenge", new { id = challenge.ChallengeID }, challenge);
        }

        // DELETE: api/Challenge/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Challenge>> DeleteChallenge(int id)
        {
            //Check of challenge bestaat
            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge == null)
            {
                return NotFound();
            }

            //Zorg ervoor dat enkel de uitdager of admin de challenge kan verwijderen terwijl deze nog niet geaccepteerd is
            int userID = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserID").Value);
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);

            if ((userID == challenge.UitdagerUserID && !challenge.Geaccepteerd) || isAdmin)
            {
                _context.Challenges.Remove(challenge);
                await _context.SaveChangesAsync();
                return challenge;
            }
            else
            {
                return Unauthorized();
            }

        }
        private bool ChallengeExists(int id)
        {
            return _context.Challenges.Any(e => e.ChallengeID == id);
        }
    }
}
