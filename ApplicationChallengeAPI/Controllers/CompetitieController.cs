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
    public class CompetitieController : ControllerBase
    {
        private readonly TafeltennisContext _context;

        public CompetitieController(TafeltennisContext context)
        {
            _context = context;
        }

        // GET: api/Competitie
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Competitie>>> GetCompetities()
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }
            return await _context.Competities.ToListAsync();
        }

        // GET: api/Competitie/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Competitie>> GetCompetitie(int id)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }
            var competitie = await _context.Competities.FindAsync(id);

            if (competitie == null)
            {
                return NotFound();
            }

            return competitie;
        }

        // PUT: api/Competitie/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompetitie(int id, Competitie competitie)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }

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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Competitie>> PostCompetitie(Competitie competitie)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }
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
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Competitie>> DeleteCompetitie(int id)
        {
            bool isAdmin = bool.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            if (!isAdmin)
            {
                return Unauthorized();
            }
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
