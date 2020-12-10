using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationChallengeAPI.Data;
using ApplicationChallengeAPI.Models;

namespace ApplicationChallengeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WedstrijdController : ControllerBase
    {
        private readonly ChallengeContext _context;

        public WedstrijdController(ChallengeContext context)
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
        // GET: api/Wedstrijd
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
        // GET: api/Wedstrijd
        [HttpGet("Betwisting")]
        public async Task<ActionResult<IEnumerable<Wedstrijd>>> GetBetwistingen()
        {
            return await _context.Wedstrijden
                .Where(w => w.Bezig == false)
                .Where(w => w.Akkoord == false)
                .Where(w => w.Team1Score == 10 || w.Team2Score == 10)
                .Include(k => k.Team1User1)
                .Include(k => k.Team1User2)
                .Include(k => k.Team2User1)
                .Include(k => k.Team2User2)
                .Include(m => m.MatchContext).ThenInclude(t => t.Tournooi)
                .Include(m => m.MatchContext).ThenInclude(c => c.Competitie)
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
                .SingleOrDefaultAsync(i => i.WedstrijdID == id);

            if (wedstrijd == null)
            {
                return NotFound();
            }

            return wedstrijd;
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

        private bool WedstrijdExists(int id)
        {
            return _context.Wedstrijden.Any(e => e.WedstrijdID == id);
        }
    }
}
