using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationChallengeAPI.Data;
using ApplicationChallengeAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationChallengeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WedstrijdController : ControllerBase
    {
        private readonly TafeltennisContext _context;

        public WedstrijdController(TafeltennisContext context)
        {
            _context = context;
        }

        // GET: api/Wedstrijd
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wedstrijd>>> GetWedstrijden()
        {
            return await _context.Wedstrijden
                .Include(k => k.Team1User1)
                .Include(k => k.Team1User2)
                .Include(k => k.Team2User1)
                .Include(k => k.Team2User2)
                .ToListAsync();
        }
        
        // GET: api/Tournooi/Wedstrijd
        [HttpGet("Tournooi/{id}")]
        public async Task<ActionResult<IEnumerable<Wedstrijd>>> GetWedstrijdenOfTournooi(int id)
        {
            return await _context.Wedstrijden.Where(w => w.MatchContext.TournooiID == id).OrderBy(w => w.MatchContext.TournooiRangschikking)
                .Include(k => k.Team1User1)
                .Include(k => k.Team1User2)
                .Include(k => k.Team2User1)
                .Include(k => k.Team2User2)
                .Include(m => m.MatchContext)
                .ToListAsync();
        }

        // GET: api/user/BonS/Wedstrijd
        [HttpGet("User/BonS/{id}")]
        public async Task<ActionResult<IEnumerable<Wedstrijd>>> GetWedstrijdenOfUserBusyOrNotStarted(int id)
        {
            return await _context.Wedstrijden.Where(w => (w.Bezig == Bezig.Bezig || w.Team1Score == 0 && w.Team2Score == 0) && //check of match bezig is of nog niet gestart is
            (w.Team1User1ID == id || w.Team1User2ID == id || w.Team2User1ID == id || w.Team2User2ID == id))//check is user is in match
                 .Include(k => k.Team1User1)
                .Include(k => k.Team1User2)
                .Include(k => k.Team2User1)
                .Include(k => k.Team2User2)
                .Include(m => m.MatchContext).ThenInclude(t => t.Tournooi)
                .Include(m => m.MatchContext).ThenInclude(c => c.Competitie)
                .Include(w => w.Team1User1.Ploeg)
                .Include(w => w.Team2User1.Ploeg)
                .Include(w => w.Tafel)
                .OrderBy(w => w.MatchContext.TournooiRangschikking)
                .ToListAsync();
        }
                
        // GET: api/Wedstrijd
        [HttpGet("Betwisting")]
        public async Task<ActionResult<IEnumerable<Wedstrijd>>> GetBetwistingen()
        {
            return await _context.Wedstrijden
                .Where(w => w.Bezig == Bezig.Gespeeld)
                .Where(w => w.Akkoord == false)
                .Where(w => w.Team1Score == 10 || w.Team2Score == 10)
                .Include(k => k.Team1User1)
                .Include(k => k.Team1User2)
                .Include(k => k.Team2User1)
                .Include(k => k.Team2User2)
                .Include(m => m.MatchContext).ThenInclude(t => t.Tournooi)
                .Include(m => m.MatchContext).ThenInclude(c => c.Competitie)
                .Include(w => w.Team1User1.Ploeg)
                .Include(w => w.Team2User1.Ploeg)
                .Include(w => w.Tafel)
                .ToListAsync();
        }

        // GET: api/Wedstrijd/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Wedstrijd>> GetWedstrijd(int id)
        {
            var wedstrijd = await _context.Wedstrijden
                .Include(k => k.Team1User1)
                .Include(k => k.Team1User2)
                .Include(k => k.Team2User1)
                .Include(k => k.Team2User2)
                .Include(m => m.MatchContext)
                .Include(t => t.MatchContext.Tournooi)
                .Include(w => w.Team1User1.Ploeg)
                .Include(w => w.Team2User1.Ploeg)
                .Include(w => w.Tafel)
                .SingleOrDefaultAsync(i => i.WedstrijdID == id);

            if (wedstrijd == null)
            {
                return NotFound();
            }

            return wedstrijd;
        }

        // GET: api/Wedstrijd/PloegStats/5
        [HttpGet("PloegStats/{id}")]
        public async Task<ActionResult<Object>> GetPloegStats(int id)
        {
            int ploegWins = await _context.Wedstrijden
                .Where(c =>
                (c.Team1User1.PloegID == id && c.Team1Score == 10) ||
                (c.Team2User1.PloegID == id && c.Team2Score == 10))
                .CountAsync();

            int ploegLosses = await _context.Wedstrijden
                .Where(c =>
                (c.Team1User1.PloegID == id && c.Team2Score == 10) ||
                (c.Team2User1.PloegID == id && c.Team1Score == 10))
                .CountAsync();
            Object stats = new { ploegWins, ploegLosses };

            return stats;
        }

        // GET: api/Wedstrijd/MijnStats
        [Authorize]
        [HttpGet("MijnStats")]
        public async Task<ActionResult<Object>> GetMijnStats()
        {
            //Check of user wel een ploeg heeft
            if (string.IsNullOrWhiteSpace(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value))
            {
                return NotFound("Je zit niet in een ploeg");
            }
            int ploegID = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "PloegID").Value);
            int userID = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserID").Value);

            int ploegWins = await _context.Wedstrijden
                .Where(c =>
                (c.Team1User1.PloegID == ploegID && c.Team1Score == 10) ||
                (c.Team2User1.PloegID == ploegID && c.Team2Score == 10))
                .CountAsync();

            int ploegLosses = await _context.Wedstrijden
                .Where(c => 
                (c.Team1User1.PloegID == ploegID && c.Team2Score == 10) ||
                (c.Team2User1.PloegID == ploegID && c.Team1Score == 10))
                .CountAsync();

            int mijnWins = await _context.Wedstrijden
                .Where(c => 
                (c.Team1User1.UserID == userID && c.Team1Score == 10) ||
                (c.Team1User2.UserID == userID && c.Team1Score == 10) ||
                (c.Team2User1.UserID == userID && c.Team2Score == 10) ||
                (c.Team2User2.UserID == userID && c.Team2Score == 10))
                .CountAsync();

            int mijnLosses = await _context.Wedstrijden
                .Where(c =>
                (c.Team1User1.UserID == userID && c.Team2Score == 10) ||
                (c.Team1User2.UserID == userID && c.Team2Score == 10) ||
                (c.Team2User1.UserID == userID && c.Team1Score == 10) ||
                (c.Team2User2.UserID == userID && c.Team1Score == 10))
                .CountAsync();


            Object stats = new { ploegWins, ploegLosses, mijnWins, mijnLosses };

            return stats;
        }

        // PUT: api/Wedstrijd/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWedstrijd(int id, Wedstrijd wedstrijd)
        {
            if (id != wedstrijd.WedstrijdID)
            {
                return BadRequest();
            }

            _context.Entry(wedstrijd).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WedstrijdExists(id))
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

        // POST: api/Wedstrijd
        [HttpPost]
        public async Task<ActionResult<Wedstrijd>> PostWedstrijd(Wedstrijd wedstrijd)
        {
            _context.Wedstrijden.Add(wedstrijd);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWedstrijd", new { id = wedstrijd.WedstrijdID }, wedstrijd);
        }

        // DELETE: api/Wedstrijd/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Wedstrijd>> DeleteWedstrijd(int id)
        {
            var wedstrijd = await _context.Wedstrijden.FindAsync(id);
            if (wedstrijd == null)
            {
                return NotFound();
            }
            var matchContext = await _context.MatchContexten.FindAsync(wedstrijd.MatchContextID);

            _context.Wedstrijden.Remove(wedstrijd);
            _context.MatchContexten.Remove(matchContext);
            await _context.SaveChangesAsync();

            return wedstrijd;
        }

        [HttpGet("User/{id}")]
        public async Task<ActionResult<IEnumerable<Wedstrijd>>> GetWedstrijdenUser(int id)
        {
            return await _context.Wedstrijden
                .Where(w => (w.Team1User1ID == id ||
                            w.Team1User2ID == id ||
                            w.Team2User1ID == id ||
                            w.Team2User2ID == id) &&
                            w.Bezig == Bezig.Gespeeld &&
                            w.Akkoord == true &&
                           (w.Team1Score != 0 || w.Team2Score != 0)
                )
                .Select( w =>
                    new Wedstrijd
                    {
                        WedstrijdID = w.WedstrijdID,
                        MatchContext = new MatchContext
                        {
                            MatchContextID = w.MatchContext.MatchContextID,
                            TournooiID = w.MatchContext.TournooiID,
                            Tournooi = w.MatchContext.Tournooi,
                            TournooiNiveau = w.MatchContext.TournooiNiveau,
                            TournooiRangschikking = w.MatchContext.TournooiRangschikking,
                            CompetitieID = w.MatchContext.CompetitieID,
                            Competitie = w.MatchContext.Competitie
                        },
                        MatchContextID = w.MatchContextID,
                        Tafel = w.Tafel,
                        TafelID = w.TafelID,
                        Team1User1ID = w.Team1User1ID,
                        Team1User1 = new User
                        {
                            UserID = w.Team1User1.UserID,
                            Naam = w.Team1User1.Naam,
                            PloegID = w.Team1User1.PloegID,
                            Ploeg = w.Team1User1.Ploeg
                        },
                        Team1User2ID = w.Team1User2ID,
                        Team1User2 = new User
                        {
                            UserID = w.Team1User2.UserID,
                            Naam = w.Team1User2.Naam
                        },
                        Team2User1ID = w.Team2User1ID,
                        Team2User1 = new User
                        {
                            UserID = w.Team2User1.UserID,
                            Naam = w.Team2User1.Naam,
                            PloegID = w.Team2User1.PloegID,
                            Ploeg = w.Team2User1.Ploeg
                        },
                        Team2User2ID = w.Team2User2ID,
                        Team2User2 = new User
                        {
                            UserID = w.Team2User2.UserID,
                            Naam = w.Team2User2.Naam
                        },
                        Team1Score = w.Team1Score,
                        Team2Score = w.Team2Score
                    }
                )
                .OrderByDescending(w => w.WedstrijdID)
                .ToListAsync();
        }

        private bool WedstrijdExists(int id)
        {
            return _context.Wedstrijden.Any(e => e.WedstrijdID == id);
        }
    }
}
