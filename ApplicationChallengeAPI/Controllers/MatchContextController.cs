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
    public class MatchContextController : ControllerBase
    {
        private readonly ChallengeContext _context;

        public MatchContextController(ChallengeContext context)
        {
            _context = context;
        }

        // GET: api/MatchContext
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchContext>>> GetMatchContexten()
        {
            return await _context.MatchContexten.Include(w => w.Wedstrijd).ToListAsync();
        }
        // GET: api/MatchContext/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchContext>> GetMatchContext(int id)
        {
            var ploeg = await _context.MatchContexten.Include(w => w.Wedstrijd).SingleOrDefaultAsync(i => i.MatchContextID == id);

            if (ploeg == null)
            {
                return NotFound();
            }

            return ploeg;
        }

        // PUT: api/MatchContext/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatchContext(int id, MatchContext ploeg)
        {
            if (id != ploeg.MatchContextID)
            {
                return BadRequest();
            }

            _context.Entry(ploeg).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchContextExists(id))
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

        // POST: api/MatchContext
        [HttpPost]
        public async Task<ActionResult<MatchContext>> PostMatchContext(MatchContext ploeg)
        {
            _context.MatchContexten.Add(ploeg);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatchContext", new { id = ploeg.MatchContextID }, ploeg);
        }

        // DELETE: api/MatchContext/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MatchContext>> DeleteMatchContext(int id)
        {
            var ploeg = await _context.MatchContexten.FindAsync(id);
            if (ploeg == null)
            {
                return NotFound();
            }

            _context.MatchContexten.Remove(ploeg);
            await _context.SaveChangesAsync();

            return ploeg;
        }

        private bool MatchContextExists(int id)
        {
            return _context.MatchContexten.Any(e => e.MatchContextID == id);
        }

        [HttpGet("User/{id}")]
        public async Task<ActionResult<IEnumerable<MatchContext>>> GetMatchContextenUser(int id)
        {
            return await _context.MatchContexten
                .Include(w => w.Wedstrijd)
                    .ThenInclude(u => u.Team1User1)
                        .ThenInclude(i => i.Ploeg)
                .Include(w => w.Wedstrijd)
                    .ThenInclude(u => u.Team1User2)
                .Include(w => w.Wedstrijd)
                    .ThenInclude(u => u.Team2User1)
                        .ThenInclude(i => i.Ploeg)
                .Include(w => w.Wedstrijd)
                    .ThenInclude(u => u.Team2User2)
                .Include(w => w.Wedstrijd)
                    .ThenInclude(t => t.Tafel)
                .Include(t => t.Tournooi)
                .Where(m => (m.Wedstrijd.Team1User1ID == id || 
                            m.Wedstrijd.Team1User2ID == id  || 
                            m.Wedstrijd.Team2User1ID == id  || 
                            m.Wedstrijd.Team2User2ID == id) &&
                            m.Wedstrijd.Bezig == false)
                .OrderByDescending(w => w.MatchContextID)
                .ToListAsync();
        }

    }
}
