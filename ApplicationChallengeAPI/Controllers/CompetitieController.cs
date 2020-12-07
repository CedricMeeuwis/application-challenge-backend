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
    public class CompetitieController : ControllerBase
    {
        private readonly ChallengeContext _context;

        public CompetitieController(ChallengeContext context)
        {
            _context = context;
        }

        // GET: api/Competitie
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Competitie>>> GetCompetities()
        {
            return await _context.Competities.ToListAsync();
        }

        // GET: api/Competitie/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Competitie>> GetCompetitie(int id)
        {
            var competitie = await _context.Competities.FindAsync(id);

            if (competitie == null)
            {
                return NotFound();
            }

            return competitie;
        }

        // PUT: api/Competitie/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompetitie(int id, Competitie competitie)
        {
            if (id != competitie.CompetitieID)
            {
                return BadRequest();
            }

            _context.Entry(competitie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetitieExists(id))
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

        // POST: api/Competitie
        [HttpPost]
        public async Task<ActionResult<Competitie>> PostCompetitie(Competitie competitie)
        {
            _context.Competities.Add(competitie);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CompetitieExists(competitie.CompetitieID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCompetitie", new { id = competitie.CompetitieID }, competitie);
        }

        // DELETE: api/Competitie/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Competitie>> DeleteCompetitie(int id)
        {
            var competitie = await _context.Competities.FindAsync(id);
            if (competitie == null)
            {
                return NotFound();
            }

            _context.Competities.Remove(competitie);
            await _context.SaveChangesAsync();

            return competitie;
        }
        private bool CompetitieExists(int id)
        {
            return _context.Competities.Any(e => e.CompetitieID == id);
        }
    }
}
